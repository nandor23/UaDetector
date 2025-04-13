using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Utils;

namespace UADetector.Parsers;

internal static class VendorFragmentParser
{
    private const string ResourceName = "Regexes.Resources.vendor_fragments.json";
    private static readonly FrozenDictionary<string, Regex[]> VendorFragments;
    private static readonly Regex CombinedRegex;


    static VendorFragmentParser()
    {
        (VendorFragments, CombinedRegex) =
            RegexLoader.LoadRegexesDictionaryWithCombined<Regex[]>(ResourceName, "[^a-z0-9]+");
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
