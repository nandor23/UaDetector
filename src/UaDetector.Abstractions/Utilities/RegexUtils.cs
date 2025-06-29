namespace UaDetector.Abstractions.Utilities;

internal static class RegexUtils
{
    public static string BuildPattern(string pattern)
    {
        return $"(?:^|[^A-Z0-9_-]|[^A-Z0-9-]_|sprd-|MZ-)(?:{pattern})";
    }
}
