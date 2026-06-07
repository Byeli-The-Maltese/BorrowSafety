// XmlRenderer.cs

using System.Text;
using Microsoft.CodeAnalysis;

namespace BorrowSafety;

internal static class XmlRenderer
{
    public static void AppendXml( StringBuilder sb, ISymbol symbol)
    {
        string? xml = symbol.GetDocumentationCommentXml();
        if (string.IsNullOrWhiteSpace(xml)) 
            return;
        string[] lines = xml!.Split('\n');
        foreach (string line in lines)
        {
            sb.Append("        /// ");
            sb.AppendLine(line.TrimEnd());
        }
    }
}
