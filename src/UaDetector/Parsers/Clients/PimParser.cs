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
    private static readonly IEnumerable<Client> PersonalInformationManagers;
    private static readonly Regex CombinedRegex;


    static PimParser()
    {
        (PersonalInformationManagers, CombinedRegex) =
            RegexLoader.LoadRegexesWithCombined<Client>(ResourceName);
    }

    public PimParser(VersionTruncation versionTruncation) : base(versionTruncation)
    {
    }

    protected override ClientType Type => ClientType.PersonalInformationManager;

    public override bool TryParse(string userAgent, ClientHints _, [NotNullWhen(true)] out ClientInfo? result)
    {
        return TryParse(userAgent, PersonalInformationManagers, CombinedRegex, out result);
    }
}
