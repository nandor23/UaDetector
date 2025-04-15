using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UaDetector.Regexes.Models;
using UaDetector.Results;
using UaDetector.Utils;

namespace UaDetector.Parsers.Devices;

internal sealed class CarBrowserParser : DeviceParserBase
{
    private const string ResourceName = "Regexes.Resources.Devices.car_browsers.json";
    private static readonly IEnumerable<Device> CarBrowsers;
    private static readonly Regex CombinedRegex;


    static CarBrowserParser()
    {
        (CarBrowsers, CombinedRegex) =
            RegexLoader.LoadRegexesWithCombined<Device>(ResourceName);
    }

    public override bool TryParse(
        string userAgent,
        [NotNullWhen(true)] out InternalDeviceInfo? result
    )
    {
        if (CombinedRegex.IsMatch(userAgent))
        {
            TryParse(userAgent, CarBrowsers, out result);
        }
        else
        {
            result = null;
        }

        return result is not null;
    }
}
