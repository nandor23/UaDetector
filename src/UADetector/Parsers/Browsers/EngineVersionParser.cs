using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Models.Constants;

namespace UADetector.Parsers.Browsers;

internal static class EngineVersionParser
{
    private static readonly Lazy<Regex> GeckoOrCleckoRegex = new(() =>
        new Regex("[ ](?:rv[: ]([0-9.]+)).*(?:g|cl)ecko/[0-9]{8,10}", RegexOptions.IgnoreCase | RegexOptions.Compiled));

    private static readonly FrozenDictionary<string, Lazy<Regex>> EngineVersionRegexes =
        new Dictionary<string, Lazy<Regex>>
        {
            { BrowserEngines.Blink, BuildRegex("Chr[o0]me|Chromium|Cronet") },
            { BrowserEngines.Arachne, BuildRegex(@"Arachne/5\.") },
            { BrowserEngines.LibWeb, BuildRegex(@"LibWeb\+LibJs") },
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    private static Lazy<Regex> BuildRegex(string pattern)
    {
        return new Lazy<Regex>(() =>
            new Regex($@"(?:{pattern})\s*[/_]?\s*((?(?=\d+\.\d)\d+[.\d]*|\d{{1,7}}(?=(?:\D|$))))",
                RegexOptions.IgnoreCase | RegexOptions.Compiled));
    }

    public static bool TryParse(string userAgent, string engine, [NotNullWhen(true)] out string? result)
    {
        Match? match = null;

        if (engine is BrowserEngines.Gecko or BrowserEngines.Clecko)
        {
            match = GeckoOrCleckoRegex.Value.Match(userAgent);
        }
        else if (EngineVersionRegexes.TryGetValue(engine, out var regex))
        {
            match = regex.Value.Match(userAgent);
        }

        result = match is not null && match.Success ? match.Groups[1].Value : null;
        return result is not null;
    }
}
