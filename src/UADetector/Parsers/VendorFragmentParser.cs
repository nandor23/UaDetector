using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace UADetector.Parsers;

internal static class VendorFragmentParser
{
    private const string ResourceName = "Regexes.Resources.vendor_fragments.yml";
    private static readonly FrozenDictionary<string, Lazy<Regex>[]> VendorFragments;
    private static readonly Lazy<Regex> CombinedRegex;


    static VendorFragmentParser()
    {
        (VendorFragments, CombinedRegex) =
            ParserExtensions.LoadRegexesDictionary<Lazy<Regex>[]>(ResourceName, "[^a-z0-9]+");
    }

    public static bool TryParseBrand(string userAgent, [NotNullWhen(true)] out string? result)
    {
        if (CombinedRegex.Value.IsMatch(userAgent))
        {
            foreach (var vendorFragment in VendorFragments)
            {
                foreach (var regex in vendorFragment.Value)
                {
                    if (regex.Value.IsMatch(userAgent))
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
