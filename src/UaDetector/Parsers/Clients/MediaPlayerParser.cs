using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Attributes;
using UaDetector.Models;
using UaDetector.Models.Enums;
using UaDetector.Models.Internal;

namespace UaDetector.Parsers.Clients;

internal sealed partial class MediaPlayerParser : ClientParserBase
{
    [RegexSource("Resources/Clients/media_players.json")]
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
