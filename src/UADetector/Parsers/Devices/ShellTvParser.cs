using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Models.Enums;
using UADetector.Regexes.Models;
using UADetector.Results;

namespace UADetector.Parsers.Devices;

internal sealed class ShellTvParser : DeviceParserBase
{
    private const string ResourceName = "Regexes.Resources.Devices.shell_televisions.yml";
    private static readonly FrozenDictionary<string, Device> ShellTelevisions;
    private static readonly Regex CombinedRegex;

    private static readonly Regex ShellTvRegex =
        ParserExtensions.BuildUserAgentRegex(@"[a-z]+[ _]Shell[ _]\w{6}|tclwebkit(\d+[.\d]*)");


    static ShellTvParser()
    {
        (ShellTelevisions, CombinedRegex) =
            ParserExtensions.LoadRegexesDictionary<Device>(ResourceName);
    }

    public override bool TryParse(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out InternalDeviceInfo? result
    )
    {
        if (ShellTvRegex.IsMatch(userAgent) && CombinedRegex.IsMatch(userAgent))
        {
            TryParse(userAgent, clientHints, ShellTelevisions, out result);

            result = new InternalDeviceInfo { Type = DeviceType.Tv, Brand = result?.Brand, Model = result?.Model, };
        }
        else
        {
            result = null;
        }

        return result is not null;
    }
}
