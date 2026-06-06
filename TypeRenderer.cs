// TypeRenderer.cs

using Microsoft.CodeAnalysis;

namespace BorrowSafety;

internal static class TypeRenderer
{
    private static readonly SymbolDisplayFormat Format = new(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions : SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions : SymbolDisplayMiscellaneousOptions.UseSpecialTypes 
            | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
    );
    
    public static string Render(ITypeSymbol type) => type.ToDisplayString(Format);
} 