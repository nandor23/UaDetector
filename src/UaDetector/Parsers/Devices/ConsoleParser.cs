using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Abstractions.Models;
using UaDetector.Attributes;
using UaDetector.Models;

namespace UaDetector.Parsers.Devices;

internal sealed partial class ConsoleParser : DeviceParserBase
{
    [RegexSource("Resources/Devices/consoles.json")]
    private static partial IReadOnlyList<Device> Consoles { get; }

    [CombinedRegex]
    private static partial Regex CombinedRegex { get; }

    public override bool TryParse(
        string userAgent,
        [NotNullWhen(true)] out DeviceInfoInternal? result
    )
    {
        if (CombinedRegex.IsMatch(userAgent))
        {
            TryParse(userAgent, Consoles, out result);
        }
        else
        {
            result = null;
        }

        return result is not null;
    }
}
