using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Abstractions.Attributes;
using UaDetector.Abstractions.Models;
using UaDetector.Results;
using UaDetector.Utilities;

namespace UaDetector.Parsers.Devices;

internal sealed partial class NotebookParser : DeviceParserBase
{
    [RegexSource("Regexes/Devices/notebooks.json")]
    private static partial IReadOnlyList<Device> Notebooks { get; }
    private static readonly Regex FbmdRegex;

    static NotebookParser()
    {
        FbmdRegex = RegexUtils.BuildUserAgentRegex("FBMD/");
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
