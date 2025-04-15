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
    private static readonly IEnumerable<Device> ShellTelevisions = RegexLoader.LoadRegexes<Device>(ResourceName);

    internal static readonly Regex ShellTvRegex =
        RegexUtility.BuildUserAgentRegex(@"[a-z]+[ _]Shell[ _]\w{6}|tclwebkit(\d+[.\d]*)");


    public override bool TryParse(
        string userAgent,
        [NotNullWhen(true)] out InternalDeviceInfo? result
    )
    {
        if (ShellTvRegex.IsMatch(userAgent))
        {
            if (!TryParse(userAgent, ShellTelevisions, out result))
            {
                result = new InternalDeviceInfo { Type = DeviceType.Tv, Brand = null, Model = null, };
            }
        }
        else
        {
            result = null;
        }

        return result is not null;
    }
}
