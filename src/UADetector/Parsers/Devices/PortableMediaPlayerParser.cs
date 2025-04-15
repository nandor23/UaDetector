using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Regexes.Models;
using UADetector.Results;
using UADetector.Utils;

namespace UADetector.Parsers.Devices;

internal sealed class PortableMediaPlayerParser : DeviceParserBase
{
    private const string ResourceName = "Regexes.Resources.Devices.portable_media_players.json";
    private static readonly IEnumerable<Device> PortableMediaPlayers;
    private static readonly Regex CombinedRegex;


    static PortableMediaPlayerParser()
    {
        (PortableMediaPlayers, CombinedRegex) =
            RegexLoader.LoadRegexesWithCombined<Device>(ResourceName);
    }

    public override bool TryParse(
        string userAgent,
        [NotNullWhen(true)] out InternalDeviceInfo? result
    )
    {
        if (CombinedRegex.IsMatch(userAgent))
        {
            TryParse(userAgent, PortableMediaPlayers, out result);
        }
        else
        {
            result = null;
        }

        return result is not null;
    }
}
