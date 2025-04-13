using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Models.Enums;
using UADetector.Regexes.Models;
using UADetector.Results;
using UADetector.Utils;

namespace UADetector.Parsers.Devices;

internal sealed class HbbTvParser : DeviceParserBase
{
    private const string ResourceName = "Regexes.Resources.Devices.televisions.json";
    private static readonly IEnumerable<Device> Televisions;
    private static readonly Regex CombinedRegex;

    internal static readonly Regex
        HbbTvRegex = RegexUtility.BuildUserAgentRegex(@"(?:HbbTV|SmartTvA)/([1-9](?:\.[0-9]){1,2})");


    static HbbTvParser()
    {
        (Televisions, CombinedRegex) =
            RegexLoader.LoadRegexesWithCombined<Device>(ResourceName);
    }

    public override bool TryParse(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out InternalDeviceInfo? result
    )
    {
        if (HbbTvRegex.IsMatch(userAgent) && CombinedRegex.IsMatch(userAgent))
        {
            TryParse(userAgent, clientHints, Televisions, out result);

            result = new InternalDeviceInfo { Type = DeviceType.Tv, Brand = result?.Brand, Model = result?.Model, };
        }
        else
        {
            result = null;
        }

        return result is not null;
    }
}
