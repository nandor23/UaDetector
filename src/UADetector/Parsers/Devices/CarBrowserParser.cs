using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Regexes.Models;
using UADetector.Results;

namespace UADetector.Parsers.Devices;

internal sealed class CarBrowserParser : BaseDeviceParser
{
    private const string ResourceName = "Regexes.Resources.Devices.car_browsers.yml";
    private static readonly FrozenDictionary<string, Device> CarBrowsers;
    private static readonly Regex CombinedRegex;


    static CarBrowserParser()
    {
        (CarBrowsers, CombinedRegex) =
            ParserExtensions.LoadRegexesDictionary<Device>(ResourceName);
    }

    public override bool TryParse(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out InternalDeviceInfo? result
    )
    {
        if (CombinedRegex.IsMatch(userAgent))
        {
            TryParse(userAgent, clientHints, CarBrowsers, out result);
        }
        else
        {
            result = null;
        }

        return result is not null;
    }
}
