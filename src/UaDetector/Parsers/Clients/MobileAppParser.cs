using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UaDetector.Attributes;
using UaDetector.Models;
using UaDetector.Models.Enums;
using UaDetector.Results;

namespace UaDetector.Parsers.Clients;

internal sealed partial class MobileAppParser : ClientParserBase
{

    [RegexSource("Regexes/Resources/Clients/mobile_apps.json")]
    internal static partial IReadOnlyList<RuleDefinition<Client>> MobileApps { get; }
    
    [CombinedRegex]
    private static partial Regex CombinedRegex { get; }
    

    public MobileAppParser(VersionTruncation versionTruncation)
        : base(versionTruncation) { }

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

        if (AppHintParser.TryParseAppName(clientHints, out var appName) && appName != name)
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
