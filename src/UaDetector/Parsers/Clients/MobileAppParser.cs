using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Models.Enums;
using UaDetector.Regexes.Models;
using UaDetector.Results;
using UaDetector.Utils;

namespace UaDetector.Parsers.Clients;

internal sealed class MobileAppParser : ClientParserBase
{
    private const string ResourceName = "Regexes.Resources.Clients.mobile_apps.json";
    public static readonly IReadOnlyList<Client> MobileApps;
    private static readonly Regex CombinedRegex;
    private readonly AppHintParser _appHintParser;

    static MobileAppParser()
    {
        (MobileApps, CombinedRegex) = RegexLoader.LoadRegexesWithCombined<Client>(ResourceName);
    }

    public MobileAppParser(VersionTruncation versionTruncation)
        : base(versionTruncation)
    {
        _appHintParser = new AppHintParser();
    }

    public override bool IsClient(string userAgent, ClientHints clientHints)
    {
        return CombinedRegex.IsMatch(userAgent) || AppHintParser.IsMobileApp(clientHints);
    }

    public override bool TryParse(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out ClientInfo? result
    )
    {
        result = null;

        TryParse(userAgent, MobileApps, CombinedRegex, out var clientInfo);

        var name = clientInfo?.Name;
        var version = clientInfo?.Version;

        if (_appHintParser.TryParseAppName(clientHints, out var appName) && appName != name)
        {
            name = appName;
            version = null;
        }

        result = name is null or { Length: 0 }
            ? null
            : new ClientInfo
            {
                Type = ClientType.MobileApp,
                Name = name,
                Version = version,
            };

        return result is not null;
    }
}
