using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Abstractions;
using UaDetector.Abstractions.Attributes;
using UaDetector.Abstractions.Enums;
using UaDetector.Abstractions.Models;
using UaDetector.Results;

namespace UaDetector.Parsers.Clients;

internal sealed partial class LibraryParser : ClientParserBase
{
    [RegexSource("Regexes/Resources/Clients/libraries.json")]
    internal static partial IReadOnlyList<RuleDefinition<Client>> Libraries { get; }

    [CombinedRegex]
    private static partial Regex CombinedRegex { get; }

    public LibraryParser(VersionTruncation versionTruncation)
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

        if (TryParse(userAgent, Libraries, CombinedRegex, out var clientInfo))
        {
            result = new ClientInfo
            {
                Type = ClientType.Library,
                Name = clientInfo.Name,
                Version = clientInfo.Version,
            };
        }

        return result is not null;
    }
}
