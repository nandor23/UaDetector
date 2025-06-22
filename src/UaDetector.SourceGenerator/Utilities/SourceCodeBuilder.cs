using Microsoft.CodeAnalysis;

using UaDetector.SourceGenerator.Models;

namespace UaDetector.SourceGenerator.Utilities;

internal static class SourceCodeBuilder
{
    public static string BuildClassSourceCode(
        RegexSourceProperty regexSourceProperty,
        string regexDeclarations,
        string collectionInitializer
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
            }
            """;
    }

    private static string ToSyntaxString(this Accessibility accessibility) =>
        accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Private => "private",
            Accessibility.Internal => "internal",
            Accessibility.Protected => "protected",
            _ => throw new NotSupportedException(),
        };
}
