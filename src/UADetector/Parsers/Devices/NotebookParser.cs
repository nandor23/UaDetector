using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Regexes.Models;
using UADetector.Results;
using UADetector.Utils;

namespace UADetector.Parsers.Devices;

internal sealed class NotebookParser : DeviceParserBase
{
    private const string ResourceName = "Regexes.Resources.Devices.notebooks.json";
    private static readonly IEnumerable<Device> Notebooks = RegexLoader.LoadRegexes<Device>(ResourceName);
    private static readonly Regex FbmdRegex = RegexUtility.BuildUserAgentRegex("FBMD/");


    public override bool TryParse(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out InternalDeviceInfo? result
    )
    {
        if (FbmdRegex.IsMatch(userAgent))
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
