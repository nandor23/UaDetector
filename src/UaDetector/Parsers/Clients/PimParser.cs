using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UaDetector.Abstractions.Models;
using UaDetector.Abstractions.Models.Enums;
using UaDetector.Abstractions.Models.Internal;
using UaDetector.Attributes;

namespace UaDetector.Parsers.Clients;

internal sealed partial class PimParser : ClientParserBase
{
    [RegexSource("Resources/Clients/pims.json")]
    internal static partial IReadOnlyList<Client> PersonalInformationManagers { get; }

    [CombinedRegex]
    private static partial Regex CombinedRegex { get; }

    public PimParser(VersionTruncation versionTruncation)
        : base(versionTruncation) { }

    public override bool IsClient(string userAgent, ClientHints _)
    {
        return CombinedRegex.IsMatch(userAgent);
    }

    public override bool TryParse(
        string userAgent,
        ClientHints _,
        [NotNullWhen(true)] out ClientInfo? result
    )
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
