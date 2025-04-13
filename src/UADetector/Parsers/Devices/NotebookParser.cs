using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Regexes.Models;
using UADetector.Results;
using UADetector.Utils;

namespace UADetector.Parsers.Devices;

internal sealed class NotebookParser : DeviceParserBase
{
    private const string ResourceName = "Regexes.Resources.Devices.notebooks.json";
    private static readonly IEnumerable<Device> Notebooks;
    private static readonly Regex CombinedRegex;


    static NotebookParser()
    {
        (Notebooks, CombinedRegex) =
            RegexLoader.LoadRegexesWithCombined<Device>(ResourceName);
    }

    public override bool TryParse(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out InternalDeviceInfo? result
    )
    {
        if (CombinedRegex.IsMatch(userAgent))
        {
            TryParse(userAgent, clientHints, Notebooks, out result);
        }
        else
        {
            result = null;
        }

        return result is not null;
    }
}
