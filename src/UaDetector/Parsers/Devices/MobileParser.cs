using System.Diagnostics.CodeAnalysis;
using UaDetector.Abstractions.Models;
using UaDetector.Attributes;
using UaDetector.Results;

namespace UaDetector.Parsers.Devices;

internal sealed partial class MobileParser : DeviceParserBase
{
    [RegexSource("Regexes/Devices/mobiles.json")]
    private static partial IReadOnlyList<Device> Mobiles { get; }

    public override bool TryParse(
        string userAgent,
        [NotNullWhen(true)] out DeviceInfoInternal? result
    )
    {
        return TryParse(userAgent, Mobiles, out result);
    }
}
