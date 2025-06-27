using System.Diagnostics.CodeAnalysis;
using UaDetector.Attributes;
using UaDetector.Models;
using UaDetector.Models.Internal;

namespace UaDetector.Parsers.Devices;

internal sealed partial class MobileParser : DeviceParserBase
{
    [RegexSource("Resources/Devices/mobiles.json")]
    private static partial IReadOnlyList<Device> Mobiles { get; }

    public override bool TryParse(
        string userAgent,
        [NotNullWhen(true)] out DeviceInfoInternal? result
    )
    {
        return TryParse(userAgent, Mobiles, out result);
    }
}
