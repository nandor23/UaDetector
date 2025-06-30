using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Abstractions.Models;
using UaDetector.Abstractions.Models.Internal;
using UaDetector.Attributes;

namespace UaDetector.Parsers.Devices;

internal sealed partial class PortableMediaPlayerParser : DeviceParserBase
{
    [RegexSource("Resources/Devices/portable_media_players.json")]
    private static partial IReadOnlyList<Device> PortableMediaPlayers { get; }

    [CombinedRegex]
    private static partial Regex CombinedRegex { get; }

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
