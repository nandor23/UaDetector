using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Models.Constants;
using UADetector.Models.Enums;
using UADetector.Regexes.Models.Client;

namespace UADetector.Parsers.Client;

internal static class EngineParser
{
    private const string ResourceName = "Regexes.Resources.Client.browser_engines.yml";

    private static readonly IEnumerable<BrowserEngine> EngineRegexes =
        ParserExtensions.LoadRegexes<BrowserEngine>(ResourceName, RegexPatternType.UserAgent);

    private static readonly FrozenSet<string> Engines = new[]
    {
        BrowserEngines.WebKit, BrowserEngines.Blink, BrowserEngines.Trident, BrowserEngines.TextBased,
        BrowserEngines.Dillo, BrowserEngines.Icab, BrowserEngines.Elektra, BrowserEngines.Presto,
        BrowserEngines.Clecko, BrowserEngines.Gecko, BrowserEngines.Khtml, BrowserEngines.NetFront,
        BrowserEngines.Edge, BrowserEngines.NetSurf, BrowserEngines.Servo, BrowserEngines.Goanna,
        BrowserEngines.EkiohFlow, BrowserEngines.Arachne, BrowserEngines.LibWeb, BrowserEngines.Maple
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    public static bool TryParse(string userAgent, [NotNullWhen(true)] out string? result)
    {
        Match? match = null;
        BrowserEngine? engine = null;

        foreach (var engineRegex in EngineRegexes)
        {
            match = engineRegex.Regex.Match(userAgent);

            if (match.Success)
            {
                engine = engineRegex;
                break;
            }
        }

        if (engine is null || match is null || !match.Success)
        {
            result = null;
            return false;
        }

        var name = ParserExtensions.FormatWithMatch(engine.Name, match);

        return Engines.TryGetValue(name, out result);
    }
}
