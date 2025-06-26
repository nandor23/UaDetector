using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Abstractions.Attributes;
using UaDetector.Abstractions.Enums;
using UaDetector.Abstractions.Models;
using UaDetector.Results;

namespace UaDetector.Parsers.Clients;

internal sealed partial class MediaPlayerParser : ClientParserBase
{
    [RegexSource("Regexes/Clients/media_players.json")]
    internal static partial IReadOnlyList<Client> MediaPlayers { get; }

    [CombinedRegex]
    private static partial Regex CombinedRegex { get; }

    public MediaPlayerParser(VersionTruncation versionTruncation)
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

        if (TryParse(userAgent, MediaPlayers, CombinedRegex, out var clientInfo))
        {
            result = new ClientInfo
            {
                Type = ClientType.MediaPlayer,
                Name = clientInfo.Name,
                Version = clientInfo.Version,
            };
        }

        return result is not null;
    }
}
