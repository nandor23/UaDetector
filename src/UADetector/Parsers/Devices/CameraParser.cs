using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Regexes.Models;
using UADetector.Results;

namespace UADetector.Parsers.Devices;

internal sealed class CameraParser : BaseDeviceParser
{
    private const string ResourceName = "Regexes.Resources.Devices.cameras.yml";
    private static readonly FrozenDictionary<string, Device> Cameras;
    private static readonly Regex CombinedRegex;


    static CameraParser()
    {
        (Cameras, CombinedRegex) =
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
            TryParse(userAgent, clientHints, Cameras, out result);
        }
        else
        {
            result = null;
        }

        return result is not null;
    }
}
