using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UaDetector.Models.Enums;
using UaDetector.Regexes.Models;
using UaDetector.Results;
using UaDetector.Utils;

namespace UaDetector.Parsers.Clients;

internal sealed class MediaPlayerParser : ClientParserBase
{
    private const string ResourceName = "Regexes.Resources.Clients.media_players.json";
    private static readonly IEnumerable<Client> MediaPlayers;
    private static readonly Regex CombinedRegex;


    static MediaPlayerParser()
    {
        (MediaPlayers, CombinedRegex) =
            RegexLoader.LoadRegexesWithCombined<Client>(ResourceName);
    }

    public MediaPlayerParser(VersionTruncation versionTruncation) : base(versionTruncation)
    {
    }

    public override bool IsClient(string userAgent, ClientHints _)
    {
        return CombinedRegex.IsMatch(userAgent);
    }

    public override bool TryParse(string userAgent, ClientHints _, [NotNullWhen(true)] out ClientInfo? result)
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
