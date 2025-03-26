using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Models.Enums;
using UADetector.Regexes.Models;
using UADetector.Results;

namespace UADetector.Parsers.Devices;

internal sealed class HbbTvParser : BaseDeviceParser
{
    private const string ResourceName = "Regexes.Resources.Devices.televisions.yml";
    private static readonly FrozenDictionary<string, Device> Televisions;
    private static readonly Regex CombinedRegex;

    private static readonly Regex
        HbbTvRegex = ParserExtensions.BuildUserAgentRegex(@"(?:HbbTV|SmartTvA)/([1-9](?:\.[0-9]){1,2})");


    static HbbTvParser()
    {
        (Televisions, CombinedRegex) =
            ParserExtensions.LoadRegexesDictionaryWithCombinedRegex<Device>(ResourceName);
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
