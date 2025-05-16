using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Models.Enums;
using UaDetector.Regexes.Models;
using UaDetector.Results;
using UaDetector.Utils;

namespace UaDetector.Parsers.Devices;

internal sealed class ShellTvParser : DeviceParserBase
{
    private const string ResourceName = "Regexes.Resources.Devices.shell_televisions.json";
    private static readonly IReadOnlyList<Device> ShellTelevisions;
    public static readonly Regex ShellTvRegex;

    static ShellTvParser()
    {
        ShellTelevisions = RegexLoader.LoadRegexes<Device>(ResourceName);
        ShellTvRegex = RegexUtility.BuildUserAgentRegex(
            @"[a-z]+[ _]Shell[ _]\w{6}|tclwebkit(\d+[.\d]*)"
        );
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
