using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Abstractions.Models;
using UaDetector.Results;
using UaDetector.Utilities;

namespace UaDetector.Parsers.Devices;

internal sealed class NotebookParser : DeviceParserBase
{
    private const string ResourceName = "Regexes.Resources.Devices.notebooks.json";
    private static readonly IReadOnlyList<Device> Notebooks;
    private static readonly Regex FbmdRegex;

    static NotebookParser()
    {
        Notebooks = RegexLoader.LoadRegexes<Device>(ResourceName);
        FbmdRegex = RegexUtilis.BuildUserAgentRegex("FBMD/");
    }

    public override bool TryParse(
        string userAgent,
        [NotNullWhen(true)] out DeviceInfoInternal? result
    )
    {
        if (FbmdRegex.IsMatch(userAgent))
        {
            TryParse(userAgent, Notebooks, out result);
        }
        else
        {
            result = null;
        }

        return result is not null;
    }
}
