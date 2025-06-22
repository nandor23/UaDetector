using UaDetector.SourceGenerator.Models;

namespace UaDetector.SourceGenerator.Utilities;

internal static class RegexBuilder
{
    public static string BuildRegexFieldDeclaration(string methodName, string pattern)
    {
        return $"""
            private static readonly global::System.Text.RegularExpressions.Regex {methodName} = 
                    new global::System.Text.RegularExpressions.Regex(
                        @"{BuildRegexPattern(pattern)}", 
                        global::System.Text.RegularExpressions.RegexOptions.IgnoreCase | 
                        global::System.Text.RegularExpressions.RegexOptions.Compiled);
            """;
    }

    public static string? BuildCombinedRegexFieldDeclaration(
        CombinedRegexProperty? combinedRegexProperty,
        string pattern
    )
    {
        if (combinedRegexProperty == null)
        {
            return null;
        }

        var fieldName = $"_{combinedRegexProperty.PropertyName}";

        return $"""
            private static readonly global::System.Text.RegularExpressions.Regex {fieldName} = 
                    new global::System.Text.RegularExpressions.Regex(
                        @"{BuildRegexPattern(pattern)}", 
                        global::System.Text.RegularExpressions.RegexOptions.IgnoreCase | 
                        global::System.Text.RegularExpressions.RegexOptions.Compiled);

                {combinedRegexProperty.PropertyAccessibility.ToSyntaxString()} static partial global::System.Text.RegularExpressions.Regex {combinedRegexProperty.PropertyName} => {fieldName}; 
            """;
    }

    private static string BuildRegexPattern(string pattern)
    {
        return $"(?:^|[^A-Z0-9_-]|[^A-Z0-9-]_|sprd-|MZ-)(?:{pattern})";
    }
}
