// ParameterRenderer.cs

using Microsoft.CodeAnalysis; 

namespace BorrowSafety;

internal static class ParameterRenderer
{ 
    public static string Render( IParameterSymbol p)
    {
        string modifier = p.RefKind switch
        {
            RefKind.Ref => "ref ",
            RefKind.Out => "out ",
            RefKind.In =>
            "in ",
            _ => ""
        };
        
        string scoped = p.ScopedKind != ScopedKind.None ? "scoped " : "";
        
        string type = TypeRenderer.Render(p.Type);
        string optional = "";
        if (p.HasExplicitDefaultValue)
        {
            optional = " = " + RenderDefault( p.ExplicitDefaultValue);
        }
        return $"{scoped}{modifier}{type} {p.Name}{optional}";
    }
    
    public static string RenderArgument( IParameterSymbol p)
    {
        string modifier = p.RefKind switch
        {
            RefKind.Ref => "ref ",
            RefKind.Out => "out ",
            RefKind.In => "in ",
            _ => ""
        };
        return modifier + p.Name;
    }
    
    private static string RenderDefault( object? value)
    {
        if (value is null) return "null";
        return value switch
        {
            string s => "\"" + s.Replace("\"", "\\\"") + "\"",
            char c => "'" + c.ToString() + "'",
            bool b => b ? "true" : "false",
            _ => value.ToString() ?? "null"
        };
    }
}

