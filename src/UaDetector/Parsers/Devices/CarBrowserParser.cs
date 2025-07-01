using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Abstractions.Models;
using UaDetector.Attributes;
using UaDetector.Models;

namespace UaDetector.Parsers.Devices;

internal sealed partial class CarBrowserParser : DeviceParserBase
{
    [RegexSource("Resources/Devices/car_browsers.json")]
    private static partial IReadOnlyList<Device> CarBrowsers { get; }

    [CombinedRegex]
    private static partial Regex CombinedRegex { get; }

    public override bool TryParse(
        string userAgent,
        [NotNullWhen(true)] out DeviceInfoInternal? result
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
