using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UaDetector.Models.Enums;
using UaDetector.Regexes.Models;
using UaDetector.Results;
using UaDetector.Utils;

namespace UaDetector.Parsers.Clients;

internal sealed class PimParser : ClientParserBase
{
    private const string ResourceName = "Regexes.Resources.Clients.pims.json";
    internal static readonly IEnumerable<Client> PersonalInformationManagers;
    private static readonly Regex CombinedRegex;


    static PimParser()
    {
        (PersonalInformationManagers, CombinedRegex) =
            RegexLoader.LoadRegexesWithCombined<Client>(ResourceName);
    }

    public PimParser(VersionTruncation versionTruncation) : base(versionTruncation)
    {
    }

    public override bool IsClient(string userAgent, ClientHints _)
    {
        return CombinedRegex.IsMatch(userAgent);
    }

    public override bool TryParse(string userAgent, ClientHints _, [NotNullWhen(true)] out ClientInfo? result)
    {
        result = null;

        if (TryParse(userAgent, PersonalInformationManagers, CombinedRegex, out var clientInfo))
        {
            result = new ClientInfo
            {
                Type = ClientType.PersonalInformationManager,
                Name = clientInfo.Name,
                Version = clientInfo.Version,
            };
        }

        return result is not null;
    }
}
