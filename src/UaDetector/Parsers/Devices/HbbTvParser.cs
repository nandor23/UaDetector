using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Models.Enums;
using UaDetector.Regexes.Models;
using UaDetector.Results;
using UaDetector.Utilities;

namespace UaDetector.Parsers.Devices;

internal sealed class HbbTvParser : DeviceParserBase
{
    private const string ResourceName = "Regexes.Resources.Devices.televisions.json";
    private static readonly IReadOnlyList<Device> Televisions;
    internal static readonly Regex HbbTvRegex;

    static HbbTvParser()
    {
        Televisions = RegexLoader.LoadRegexes<Device>(ResourceName);
        HbbTvRegex = RegexUtilis.BuildUserAgentRegex(
            @"(?:HbbTV|SmartTvA)/([1-9](?:\.[0-9]){1,2})"
        );
    }

    public override bool TryParse(
        string userAgent,
        [NotNullWhen(true)] out DeviceInfoInternal? result
    )
    {
        if (HbbTvRegex.IsMatch(userAgent))
        {
            if (!TryParse(userAgent, Televisions, out result))
            {
                result = new DeviceInfoInternal
                {
                    Type = DeviceType.Television,
                    Brand = null,
                    Model = null,
                };
            }
        }
        else
        {
            result = null;
        }

        return result is not null;
    }
}
