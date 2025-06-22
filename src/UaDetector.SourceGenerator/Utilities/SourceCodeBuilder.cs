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

        return $$"""
            {{regexSourceProperty.Namespace}}

            partial class {{regexSourceProperty.ContainingClass}}
            {
                {{regexDeclarations}}
                
                private static readonly global::System.Collections.Generic.IReadOnlyList<{{regexSourceProperty.ElementType}}> {{fieldName}} = {{collectionInitializer}};

                {{regexSourceProperty.PropertyAccessibility.ToSyntaxString()}} static partial global::System.Collections.Generic.IReadOnlyList<{{regexSourceProperty.ElementType}}> {{regexSourceProperty.PropertyName}} =>
                    {{fieldName}};

                {{combinedRegexDeclaration ?? string.Empty}}
            }
            """;
    }
}
