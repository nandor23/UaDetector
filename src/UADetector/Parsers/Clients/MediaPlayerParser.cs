using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Models.Enums;
using UADetector.Regexes.Models;
using UADetector.Results;
using UADetector.Utils;

namespace UADetector.Parsers.Clients;

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

    protected override ClientType Type => ClientType.MediaPlayer;

    public override bool TryParse(string userAgent, ClientHints _, [NotNullWhen(true)] out ClientInfo? result)
    {
        return TryParse(userAgent, MediaPlayers, CombinedRegex, out result);
    }
}
