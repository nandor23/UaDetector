using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Regexes.Models;
using UaDetector.Results;
using UaDetector.Utils;

namespace UaDetector.Parsers.Devices;

internal sealed class CameraParser : DeviceParserBase
{
    private const string ResourceName = "Regexes.Resources.Devices.cameras.json";
    private static readonly IReadOnlyList<Device> Cameras;
    private static readonly Regex CombinedRegex;

    static CameraParser()
    {
        (Cameras, CombinedRegex) = RegexLoader.LoadRegexesWithCombined<Device>(ResourceName);
    }

    public override bool TryParse(
        string userAgent,
        [NotNullWhen(true)] out DeviceInfoInternal? result
    )
    {
        if (CombinedRegex.IsMatch(userAgent))
        {
            TryParse(userAgent, Cameras, out result);
        }
        else
        {
            result = null;
        }

        return result is not null;
    }
}
