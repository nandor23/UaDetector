using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Models.Constants;
using UaDetector.Regexes.Models.Browsers;
using UaDetector.Utils;

namespace UaDetector.Parsers.Browsers;

internal sealed class EngineParser
{
    private const string ResourceName = "Regexes.Resources.Browsers.browser_engines.json";
    private static readonly IReadOnlyList<BrowserEngine> Engines;
    private static readonly Regex CombinedRegex;
    private readonly ParserHelper _parserHelper = new();

    public static readonly FrozenSet<string> EngineNames = new[]
    {
        BrowserEngines.WebKit,
        BrowserEngines.Blink,
        BrowserEngines.Trident,
        BrowserEngines.TextBased,
        BrowserEngines.Dillo,
        BrowserEngines.Icab,
        BrowserEngines.Elektra,
        BrowserEngines.Presto,
        BrowserEngines.Clecko,
        BrowserEngines.Gecko,
        BrowserEngines.Khtml,
        BrowserEngines.NetFront,
        BrowserEngines.Edge,
        BrowserEngines.NetSurf,
        BrowserEngines.Servo,
        BrowserEngines.Goanna,
        BrowserEngines.EkiohFlow,
        BrowserEngines.Arachne,
        BrowserEngines.LibWeb,
        BrowserEngines.Maple,
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    static EngineParser()
    {
        (Engines, CombinedRegex) = RegexLoader.LoadRegexesWithCombined<BrowserEngine>(ResourceName);
    }

    public bool TryParse(string userAgent, [NotNullWhen(true)] out string? result)
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

        var name = _parserHelper.FormatWithMatch(engine.Name, match);

        return EngineNames.TryGetValue(name, out result);
    }
}
