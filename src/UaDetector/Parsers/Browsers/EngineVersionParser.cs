using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Abstractions.Constants;

namespace UaDetector.Parsers.Browsers;

internal static class EngineVersionParser
{
    private static readonly Regex GeckoOrCleckoRegex = new(
        "[ ](?:rv[: ]([0-9.]+)).*(?:g|cl)ecko/[0-9]{8,10}",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    private static readonly FrozenDictionary<string, Regex> EngineVersionRegexes = new Dictionary<
        string,
        Regex
    >
    {
        { BrowserEngines.Blink, BuildRegex("Chr[o0]me|Chromium|Cronet") },
        { BrowserEngines.Arachne, BuildRegex(@"Arachne/5\.") },
        { BrowserEngines.LibWeb, BuildRegex(@"LibWeb\+LibJs") },
        { BrowserEngines.WebKit, BuildRegex(BrowserEngines.WebKit) },
        { BrowserEngines.Trident, BuildRegex(BrowserEngines.Trident) },
        { BrowserEngines.TextBased, BuildRegex(BrowserEngines.TextBased) },
        { BrowserEngines.Dillo, BuildRegex(BrowserEngines.Dillo) },
        { BrowserEngines.Icab, BuildRegex(BrowserEngines.Icab) },
        { BrowserEngines.Elektra, BuildRegex(BrowserEngines.Elektra) },
        { BrowserEngines.Presto, BuildRegex(BrowserEngines.Presto) },
        { BrowserEngines.Clecko, BuildRegex(BrowserEngines.Clecko) },
        { BrowserEngines.Gecko, BuildRegex(BrowserEngines.Gecko) },
        { BrowserEngines.Khtml, BuildRegex(BrowserEngines.Khtml) },
        { BrowserEngines.NetFront, BuildRegex(BrowserEngines.NetFront) },
        { BrowserEngines.Edge, BuildRegex(BrowserEngines.Edge) },
        { BrowserEngines.NetSurf, BuildRegex(BrowserEngines.NetSurf) },
        { BrowserEngines.Servo, BuildRegex(BrowserEngines.Servo) },
        { BrowserEngines.Goanna, BuildRegex(BrowserEngines.Goanna) },
        { BrowserEngines.EkiohFlow, BuildRegex(BrowserEngines.EkiohFlow) },
        { BrowserEngines.Maple, BuildRegex(BrowserEngines.Maple) },
    }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    private static Regex BuildRegex(string pattern)
    {
        return new Regex(
            $@"(?:{pattern})\s*[/_]?\s*((?(?=\d+\.\d)\d+[.\d]*|\d{{1,7}}(?=(?:\D|$))))",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );
    }

    public static bool TryParse(
        string userAgent,
        string engine,
        [NotNullWhen(true)] out string? result
    )
    {
        Match? match = null;

        if (engine is BrowserEngines.Gecko or BrowserEngines.Clecko)
        {
            match = GeckoOrCleckoRegex.Match(userAgent);
        }

        if (
            (match is null || !match.Success)
            && EngineVersionRegexes.TryGetValue(engine, out var regex)
        )
        {
            match = regex.Match(userAgent);
        }

        result = match is not null && match.Success ? match.Groups[1].Value : null;
        return result is not null;
    }
}
