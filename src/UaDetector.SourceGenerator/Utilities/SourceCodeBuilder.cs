using Microsoft.CodeAnalysis;

namespace UaDetector.SourceGenerator.Utilities;

internal static class SourceCodeBuilder
{
    public static string BuildClassSourceCode(PropertyDeclarationInfo property, string regexDeclarations, string collectionInitializer)
    {
        var fieldName = $"_{property.PropertyName}";

        return $$"""
                 {{property.Namespace}}

                 partial class {{property.ContainingClass}}
                 {
                     {{regexDeclarations}}
                     
                     private static readonly global::System.Collections.Generic.IReadOnlyList<{{property.ElementType}}> {{fieldName}} = {{collectionInitializer}};

                     {{property.PropertyAccessibility.ToSyntaxString()}} static partial global::System.Collections.Generic.IReadOnlyList<{{property.ElementType}}> {{property.PropertyName}} =>
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
