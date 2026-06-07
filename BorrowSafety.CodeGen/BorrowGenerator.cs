// BorrowGenerator.cs

using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BorrowSafety;

/// <summary>
/// The generator that forwards public methods and properties on a reference type
/// to any Borrow that uses the reference type as its type parameter T
/// </summary>
[Generator]
public sealed class BorrowGenerator : IIncrementalGenerator 
{
    /// <summary>
    /// Initializes the generator
    /// </summary>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    { 
        static bool SyntaxProviderPredicate(SyntaxNode node, CancellationToken ct) 
            => node is GenericNameSyntax g && g.Identifier.Text == "Borrow";
            
        static INamedTypeSymbol? GetBorrowTarget(GeneratorSyntaxContext context, CancellationToken ct)
        {
            GenericNameSyntax syntax = (GenericNameSyntax)context.Node;
            SymbolInfo info = context.SemanticModel.GetSymbolInfo(syntax);
            if (info.Symbol is not INamedTypeSymbol symbol) return null;
            INamedTypeSymbol? borrowType = context.SemanticModel.Compilation.GetTypeByMetadataName("BorrowSafety.Borrow`1");
            if (borrowType is null) return null;
            if (!SymbolEqualityComparer.Default.Equals(symbol.ConstructedFrom, borrowType)) return null;
            if (symbol.TypeArguments.Length != 1) return null;
            return symbol.TypeArguments[0] as INamedTypeSymbol;
        } 
        
        static INamedTypeSymbol AssertNotNull(INamedTypeSymbol? definitelyNotNull, CancellationToken ct) 
            => definitelyNotNull!;
            
        IncrementalValueProvider<ImmutableArray<INamedTypeSymbol>> collected = context
            .SyntaxProvider
            .CreateSyntaxProvider(SyntaxProviderPredicate, GetBorrowTarget)
            .Where(static x => x is not null)
            .Select(AssertNotNull)
            .Collect();
            
        static void Registration(SourceProductionContext spc, ImmutableArray<INamedTypeSymbol> symbols)
        {
            HashSet<INamedTypeSymbol> unique = new(SymbolEqualityComparer.Default);
            foreach (INamedTypeSymbol symbol in symbols)
            {
                if (!unique.Add(symbol)) continue;
                string source = BorrowEmitter.Emit(symbol);
                spc.AddSource($"{SafeName(symbol)}_BorrowExtensions.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }
        
        context.RegisterSourceOutput(collected, Registration);
    }
    
    internal static string SafeName(ITypeSymbol type) => type
        .ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
        .Replace("<", "_")
        .Replace(">", "")
        .Replace(",", "_")
        .Replace(".", "_")
        .Replace(" ", "")
        .Replace("?", "Nullable")
        ;
        
}

