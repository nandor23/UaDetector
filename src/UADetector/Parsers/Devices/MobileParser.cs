using System.Diagnostics.CodeAnalysis;

using UADetector.Regexes.Models;
using UADetector.Results;
using UADetector.Utils;

namespace UADetector.Parsers.Devices;

internal sealed class MobileParser : DeviceParserBase
{
    private const string ResourceName = "Regexes.Resources.Devices.mobiles.json";
    private static readonly IEnumerable<Device> Mobiles = RegexLoader.LoadRegexes<Device>(ResourceName);


    public override bool TryParse(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out InternalDeviceInfo? result
    )
    {
        return TryParse(userAgent, clientHints, Mobiles, out result);
    }
}
