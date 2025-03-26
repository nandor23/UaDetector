using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Regexes.Models;
using UADetector.Results;

namespace UADetector.Parsers.Devices;

internal sealed class NotebookParser : BaseDeviceParser
{
    private const string ResourceName = "Regexes.Resources.Devices.notebooks.yml";
    private static readonly FrozenDictionary<string, Device> Notebooks;
    private static readonly Regex CombinedRegex;
    private static readonly Regex FbmdRegex = ParserExtensions.BuildUserAgentRegex("FBMD/");


    static NotebookParser()
    {
        (Notebooks, CombinedRegex) =
            ParserExtensions.LoadRegexesDictionaryWithCombinedRegex<Device>(ResourceName);
    }

    public override bool TryParse(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out InternalDeviceInfo? result
    )
    {
        if (FbmdRegex.IsMatch(userAgent) && CombinedRegex.IsMatch(userAgent))
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
