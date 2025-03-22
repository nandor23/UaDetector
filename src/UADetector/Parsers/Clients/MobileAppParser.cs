using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Models.Enums;
using UADetector.Regexes.Models;
using UADetector.Results;

namespace UADetector.Parsers.Clients;

internal sealed class MobileAppParser : BaseClientParser
{
    private const string ResourceName = "Regexes.Resources.Clients.mobile_apps.yml";
    private static readonly IEnumerable<Client> MobileApps;
    private static readonly Regex OverallRegex;


    static MobileAppParser()
    {
        (MobileApps, OverallRegex) =
            ParserExtensions.LoadRegexesWithOverallRegex<Client>(ResourceName, RegexPatternType.UserAgent);
    }

    public MobileAppParser(VersionTruncation versionTruncation) : base(versionTruncation)
    {
    }

    protected override ClientType Type => ClientType.MobileApp;

    public override bool TryParse(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out ClientInfo? result
    )
    {
        if (TryParse(userAgent, MobileApps, OverallRegex, out result))
        {
            var name = result.Name;
            var version = result.Version;

            if (AppHintParser.TryParseAppName(clientHints, out var appName) && appName != name)
            {
                name = appName;
                version = null;
            }

            result = new ClientInfo { Type = result.Type, Name = name, Version = version, };
            return true;
        }

        result = null;
        return false;
    }
}
