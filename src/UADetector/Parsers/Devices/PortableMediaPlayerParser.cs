using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Regexes.Models;
using UADetector.Results;

namespace UADetector.Parsers.Devices;

internal sealed class PortableMediaPlayerParser : BaseDeviceParser
{
    private const string ResourceName = "Regexes.Resources.Devices.portable_media_players.yml";
    private static readonly FrozenDictionary<string, Device> PortableMediaPlayers;
    private static readonly Lazy<Regex> CombinedRegex;


    static PortableMediaPlayerParser()
    {
        (PortableMediaPlayers, CombinedRegex) =
            ParserExtensions.LoadRegexesDictionary<Device>(ResourceName);
    }

    public override bool TryParse(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out InternalDeviceInfo? result
    )
    {
        if (CombinedRegex.Value.IsMatch(userAgent))
        {
            TryParse(userAgent, clientHints, PortableMediaPlayers, out result);
        }
        else
        {
            result = null;
        }

        return result is not null;
    }
}
