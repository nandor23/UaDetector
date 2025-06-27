using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UaDetector.Attributes;
using UaDetector.Models.Constants;
using UaDetector.Models.Internal;

namespace UaDetector.Parsers.Browsers;

internal static partial class EngineParser
{
    [RegexSource("Resources/Browsers/browser_engines.json")]
    private static partial IReadOnlyList<Engine> Engines { get; }

    [CombinedRegex]
    private static partial Regex CombinedRegex { get; }

    internal static readonly FrozenSet<string> EngineNames = new[]
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

    public static bool TryParse(string userAgent, [NotNullWhen(true)] out string? result)
    {
        if (!CombinedRegex.IsMatch(userAgent))
        {
            result = null;
            return false;
        }

        Match? match = null;
        Engine? engine = null;

        foreach (var browserEngine in Engines)
        {
            match = browserEngine.Regex.Match(userAgent);

            if (match.Success)
            {
                engine = browserEngine;
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
