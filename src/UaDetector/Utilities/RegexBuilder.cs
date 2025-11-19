using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace UaDetector.Utilities;

internal static class RegexBuilder
{
    public static Regex BuildRegex([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
    {
        return new Regex(
            $"(?:^|[^A-Z0-9_-]|[^A-Z0-9-]_|sprd-|MZ-)(?:{pattern})",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );
    }
}
