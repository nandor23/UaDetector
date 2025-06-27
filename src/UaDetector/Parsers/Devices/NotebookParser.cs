using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Attributes;
using UaDetector.Models;
using UaDetector.Models.Internal;
using UaDetector.Utilities;

namespace UaDetector.Parsers.Devices;

internal sealed partial class NotebookParser : DeviceParserBase
{
    [RegexSource("Resources/Devices/notebooks.json")]
    private static partial IReadOnlyList<Device> Notebooks { get; }
    private static readonly Regex FbmdRegex;

    static NotebookParser()
    {
        FbmdRegex = RegexBuilder.BuildRegex("FBMD/");
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
