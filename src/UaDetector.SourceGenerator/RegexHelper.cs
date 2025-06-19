namespace UaDetector.SourceGenerator;

internal static class RegexHelper
{
    public static string BuildRegexFieldDeclaration(string methodName, string pattern)
    {
        return $"""
            public static readonly global::System.Text.RegularExpressions.Regex {methodName} = 
                new global::System.Text.RegularExpressions.Regex(
                    @"{BuildRegexPattern(pattern)}", 
                    global::System.Text.RegularExpressions.RegexOptions.IgnoreCase | 
                    global::System.Text.RegularExpressions.RegexOptions.Compiled);
            """;
    }

    private static string BuildRegexPattern(string pattern)
    {
        return $"(?:^|[^A-Z0-9_-]|[^A-Z0-9-]_|sprd-|MZ-)(?:{pattern})";
    }
}
