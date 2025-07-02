using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Abstractions.Enums;
using UaDetector.Abstractions.Models;
using UaDetector.Attributes;
using UaDetector.Models;
using UaDetector.Utilities;

namespace UaDetector.Parsers.Devices;

internal sealed partial class ShellTvParser : DeviceParserBase
{
    [RegexSource("Resources/Devices/shell_televisions.json")]
    private static partial IReadOnlyList<Device> ShellTelevisions { get; }
    internal static readonly Regex ShellTvRegex;

    static ShellTvParser()
    {
        ShellTvRegex = RegexBuilder.BuildRegex(@"[a-z]+[ _]Shell[ _]\w{6}|tclwebkit(\d+[.\d]*)");
    }

    public override bool TryParse(
        string userAgent,
        [NotNullWhen(true)] out DeviceInfoInternal? result
    )
    {
        if (ShellTvRegex.IsMatch(userAgent))
        {
            if (!TryParse(userAgent, ShellTelevisions, out result))
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
