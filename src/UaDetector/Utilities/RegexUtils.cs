using System.Text.RegularExpressions;

namespace UaDetector.Utilities;

internal static class RegexUtils
{
    public static Regex BuildUserAgentRegex(string pattern)
    {
        return new Regex(
            $"(?:^|[^A-Z0-9_-]|[^A-Z0-9-]_|sprd-|MZ-)(?:{pattern})",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );
    }
}
