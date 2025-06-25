namespace UaDetector.SourceGenerator.Utilities;

internal static class StringExtensions
{
    public static string? NullIf(this string value, string check) =>
        value.Equals(check, StringComparison.Ordinal) ? null : value;

    public static string EscapeStringLiteral(this string value)
    {
        return value
            .Replace("\\", @"\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }
}
