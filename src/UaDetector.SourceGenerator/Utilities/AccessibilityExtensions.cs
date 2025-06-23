using Microsoft.CodeAnalysis;

namespace UaDetector.SourceGenerator.Utilities;

internal static class AccessibilityExtensions
{
    public static string ToSyntaxString(this Accessibility accessibility) =>
        accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Private => "private",
            Accessibility.Internal => "internal",
            Accessibility.Protected => "protected",
            _ => throw new NotSupportedException(),
        };
}
