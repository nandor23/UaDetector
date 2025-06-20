namespace UaDetector.SourceGenerator.Utilities;

internal static class StringExtensions
{
    public static string? NullIf(this string value, string check) =>
        value.Equals(check, StringComparison.Ordinal) ? null : value;
}
