using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Models.Constants;
using UADetector.Regexes.Models.Browsers;
using UADetector.Utils;

namespace UADetector.Parsers.Browsers;

internal static class EngineParser
{
    private const string ResourceName = "Regexes.Resources.Browsers.browser_engines.json";
    private static readonly IEnumerable<BrowserEngine> Engines;
    private static readonly Regex CombinedRegex;

    private static readonly FrozenSet<string> EngineNames = new[]
    {
        BrowserEngines.WebKit, BrowserEngines.Blink, BrowserEngines.Trident, BrowserEngines.TextBased,
        BrowserEngines.Dillo, BrowserEngines.Icab, BrowserEngines.Elektra, BrowserEngines.Presto,
        BrowserEngines.Clecko, BrowserEngines.Gecko, BrowserEngines.Khtml, BrowserEngines.NetFront,
        BrowserEngines.Edge, BrowserEngines.NetSurf, BrowserEngines.Servo, BrowserEngines.Goanna,
        BrowserEngines.EkiohFlow, BrowserEngines.Arachne, BrowserEngines.LibWeb, BrowserEngines.Maple
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    static EngineParser()
    {
        (Engines, CombinedRegex) = RegexLoader.LoadRegexesWithCombined<BrowserEngine>(ResourceName);
    }

    public static bool TryParse(string userAgent, [NotNullWhen(true)] out string? result)
    {

        if (!CombinedRegex.IsMatch(userAgent))
        {
            result = null;
            return false;
        }

        Match? match = null;
        BrowserEngine? engine = null;

        foreach (var enginePattern in Engines)
        {
            match = enginePattern.Regex.Match(userAgent);

            if (match.Success)
            {
                engine = enginePattern;
                break;
            }
        }

        if (engine is null || match is null || !match.Success)
        {
            result = null;
            return false;
        }

        var name = ParserExtensions.FormatWithMatch(engine.Name, match);

        return EngineNames.TryGetValue(name, out result);
    }
}
