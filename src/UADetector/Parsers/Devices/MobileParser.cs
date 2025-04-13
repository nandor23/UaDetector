using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Regexes.Models;
using UADetector.Results;
using UADetector.Utils;

namespace UADetector.Parsers.Devices;

internal sealed class MobileParser : DeviceParserBase
{
    private const string ResourceName = "Regexes.Resources.Devices.mobiles.json";
    private static readonly IEnumerable<Device> Mobiles;
    private static readonly Regex CombinedRegex;


    static MobileParser()
    {
        (Mobiles, CombinedRegex) =
            RegexLoader.LoadRegexesWithCombined<Device>(ResourceName);
    }

    public override bool TryParse(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out InternalDeviceInfo? result
    )
    {
        if (CombinedRegex.IsMatch(userAgent))
        {
            TryParse(userAgent, clientHints, Mobiles, out result);
        }
        else
        {
            result = null;
        }

        return result is not null;
    }
}
