using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Abstractions.Models;
using UaDetector.Results;
using UaDetector.Utilities;

namespace UaDetector.Parsers.Devices;

internal sealed class PortableMediaPlayerParser : DeviceParserBase
{
    private const string ResourceName = "Regexes.Resources.Devices.portable_media_players.json";
    private static readonly IReadOnlyList<Device> PortableMediaPlayers;
    private static readonly Regex CombinedRegex;

    static PortableMediaPlayerParser()
    {
        (PortableMediaPlayers, CombinedRegex) = RegexLoader.LoadRegexesWithCombined<Device>(
            ResourceName
        );
    }

    public override bool TryParse(
        string userAgent,
        [NotNullWhen(true)] out DeviceInfoInternal? result
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
