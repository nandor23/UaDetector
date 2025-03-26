using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace UADetector.Parsers;

internal static class VendorFragmentParser
{
    private const string ResourceName = "Regexes.Resources.vendor_fragments.yml";
    private static readonly FrozenDictionary<string, Regex[]> VendorFragments;
    private static readonly Regex CombinedRegex;


    static VendorFragmentParser()
    {
        (VendorFragments, CombinedRegex) =
            ParserExtensions.LoadRegexesDictionary<Regex[]>(ResourceName, "[^a-z0-9]+");
    }

    public static bool TryParseBrand(string userAgent, [NotNullWhen(true)] out string? result)
    {
        if (CombinedRegex.IsMatch(userAgent))
        {
            foreach (var vendorFragment in VendorFragments)
            {
                foreach (var regex in vendorFragment.Value)
                {
                    if (regex.IsMatch(userAgent))
                    {
                        result = vendorFragment.Key;
                        return true;
                    }
                }
            }
        }

        result = null;
        return false;
    }

}
