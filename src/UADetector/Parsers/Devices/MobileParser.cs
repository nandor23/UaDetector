using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Regexes.Models;
using UADetector.Results;

namespace UADetector.Parsers.Devices;

internal sealed class MobileParser : BaseDeviceParser
{
    private const string ResourceName = "Regexes.Resources.Devices.mobiles.yml";
    private static readonly FrozenDictionary<string, Device> Mobiles;
    private static readonly Regex CombinedRegex;


    static MobileParser()
    {
        (Mobiles, CombinedRegex) =
            ParserExtensions.LoadRegexesDictionaryWithCombinedRegex<Device>(ResourceName);
    }

    public override bool TryParse(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out InternalDeviceInfo? result
    )
    {
        if (CombinedRegex.IsMatch(userAgent))
        {
            TryParse(userAgent, clientHints, Mobiles, out result);
        }
        else
        {
            result = null;
        }

        return result is not null;
    }
}
