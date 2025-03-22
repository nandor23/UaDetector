using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Models.Enums;
using UADetector.Regexes.Models;
using UADetector.Results;

namespace UADetector.Parsers.Clients;

internal sealed class PimParser : BaseClientParser
{
    private const string ResourceName = "Regexes.Resources.Clients.pims.yml";
    private static readonly IEnumerable<Client> PersonalInformationManagers;
    private static readonly Regex CombinedRegex;


    static PimParser()
    {
        (PersonalInformationManagers, CombinedRegex) =
            ParserExtensions.LoadRegexesWithCombinedRegex<Client>(ResourceName, RegexPatternType.UserAgent);
    }

    public PimParser(VersionTruncation versionTruncation) : base(versionTruncation)
    {
    }

    protected override ClientType Type => ClientType.PIM;

    public override bool TryParse(string userAgent, ClientHints _, [NotNullWhen(true)] out ClientInfo? result)
    {
        return TryParse(userAgent, PersonalInformationManagers, CombinedRegex, out result);
    }
}
