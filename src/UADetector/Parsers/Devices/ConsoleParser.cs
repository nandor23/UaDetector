using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Regexes.Models;
using UADetector.Results;

namespace UADetector.Parsers.Devices;

internal sealed class ConsoleParser : BaseDeviceParser
{
    private const string ResourceName = "Regexes.Resources.Devices.consoles.yml";
    private static readonly FrozenDictionary<string, Device> Consoles;
    private static readonly Regex CombinedRegex;


    static ConsoleParser()
    {
        (Consoles, CombinedRegex) =
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
            TryParse(userAgent, clientHints, Consoles, out result);
        }
        else
        {
            result = null;
        }

        return result is not null;
    }
}
