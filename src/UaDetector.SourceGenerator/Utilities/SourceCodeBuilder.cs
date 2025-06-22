using System.Text;
using UaDetector.SourceGenerator.Models;

namespace UaDetector.SourceGenerator.Utilities;

internal static class SourceCodeBuilder
{
    public static string BuildClassSourceCode(
        RegexSourceProperty regexSourceProperty,
        string regexDeclarations,
        string collectionInitializer,
        string? combinedRegexDeclaration
    )
    {
        var fieldName = $"_{regexSourceProperty.PropertyName}";
        var classModifier = regexSourceProperty.IsStaticClass ? "static partial" : "partial";

        var sb = new StringBuilder();

        sb.AppendLine(
            $$"""
            {{regexSourceProperty.Namespace}}

            {{classModifier}} class {{regexSourceProperty.ContainingClass}}
            {
                {{regexDeclarations}}
                
                private static readonly global::System.Collections.Generic.IReadOnlyList<{{regexSourceProperty.ElementType}}> {{fieldName}} = {{collectionInitializer}};

                {{regexSourceProperty.PropertyAccessibility.ToSyntaxString()}} static partial global::System.Collections.Generic.IReadOnlyList<{{regexSourceProperty.ElementType}}> {{regexSourceProperty.PropertyName}} => {{fieldName}};
            """
        );

        if (combinedRegexDeclaration is not null)
        {
            sb.AppendLine();
            sb.AppendLine($"    {combinedRegexDeclaration}");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }
}
