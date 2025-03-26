using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace UADetector.Parsers;

internal static class VendorFragmentParser
{
    private const string ResourceName = "Regexes.Resources.vendor_fragments.yml";

    private static readonly FrozenDictionary<string, Regex[]> VendorFragments =
        ParserExtensions.LoadRegexesDictionary<Regex[]>(ResourceName, "[^a-z0-9]+");


    public static bool TryParseBrand(string userAgent, [NotNullWhen(true)] out string? result)
    {
        foreach (var vendor in VendorFragments)
        {
            foreach (var regex in vendor.Value)
            {
                if (regex.IsMatch(userAgent))
                {
                    result = vendor.Key;
                    return true;
                }
            }
        }

        result = null;
        return false;
    }

}
