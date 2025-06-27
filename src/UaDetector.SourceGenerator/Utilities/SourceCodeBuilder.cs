using System.Text;

using UaDetector.SourceGenerator.Models;

namespace UaDetector.SourceGenerator.Utilities;

internal static class SourceCodeBuilder
{
    public static string BuildClassSourceCode(
        RegexSourceProperty property,
        string regexDeclarations,
        string collectionInitializer,
        string? combinedRegexDeclaration
    )
    {
        var fieldName = $"_{property.PropertyName}";
        var classModifier = property.IsStaticClass ? "static partial" : "partial";

        var sb = new StringBuilder();

        sb.AppendLine(
            $$"""
            {{property.Namespace}}

            {{classModifier}} class {{property.ContainingClass}}
            {
                {{regexDeclarations}}
                
                private static readonly global::System.Collections.Generic.IReadOnlyList<{{property.ElementType}}> {{fieldName}} = {{collectionInitializer}};

                {{property.PropertyAccessibility.ToSyntaxString()}} static partial global::System.Collections.Generic.IReadOnlyList<{{property.ElementType}}> {{property.PropertyName}} => {{fieldName}};
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
