using System.Text.RegularExpressions;
using UaDetector.Abstractions.Utilities;

namespace UaDetector.Utilities;

internal static class RegexBuilder
{
    public static Regex BuildRegex(string pattern)
    {
        return new Regex(
            RegexUtils.BuildPattern(pattern),
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );
    }
}
