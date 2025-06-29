using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UaDetector.Abstractions.Models;
using UaDetector.Abstractions.Models.Enums;
using UaDetector.Abstractions.Models.Internal;
using UaDetector.Attributes;
using UaDetector.Utilities;

namespace UaDetector.Parsers.Devices;

internal sealed partial class HbbTvParser : DeviceParserBase
{
    [RegexSource("Resources/Devices/televisions.json")]
    private static partial IReadOnlyList<Device> Televisions { get; }
    internal static readonly Regex HbbTvRegex;

    static HbbTvParser()
    {
        HbbTvRegex = RegexBuilder.BuildRegex(@"(?:HbbTV|SmartTvA)/([1-9](?:\.[0-9]){1,2})");
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
