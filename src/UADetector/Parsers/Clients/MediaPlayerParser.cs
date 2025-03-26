using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Models.Enums;
using UADetector.Regexes.Models;
using UADetector.Results;

namespace UADetector.Parsers.Clients;

internal sealed class MediaPlayerParser : BaseClientParser
{
    private const string ResourceName = "Regexes.Resources.Clients.media_players.yml";
    private static readonly IEnumerable<Client> MediaPlayers;
    private static readonly Regex CombinedRegex;


    static MediaPlayerParser()
    {
        (MediaPlayers, CombinedRegex) =
            ParserExtensions.LoadRegexes<Client>(ResourceName);
    }

    public MediaPlayerParser(VersionTruncation versionTruncation) : base(versionTruncation)
    {
    }

    protected override ClientType Type => ClientType.MediaPlayer;

    public override bool TryParse(string userAgent, ClientHints _, [NotNullWhen(true)] out ClientInfo? result)
    {
        return TryParse(userAgent, MediaPlayers, CombinedRegex, out result);
    }
}
