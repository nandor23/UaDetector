using System.Diagnostics.CodeAnalysis;
using UaDetector.Abstractions.Attributes;
using UaDetector.Abstractions.Models;

namespace UaDetector.Parsers;

internal static partial class VendorFragmentParser
{
    [RegexSource("Regexes/vendor_fragments.json", "[^a-z0-9]+")]
    internal static partial IReadOnlyList<VendorFragment> VendorFragments { get; }

    public static bool TryParseBrand(string userAgent, [NotNullWhen(true)] out string? result)
    {
        foreach (var vendorFragment in VendorFragments)
        {
            foreach (var regex in vendorFragment.Regexes)
            {
                if (regex.IsMatch(userAgent))
                {
                    result = vendorFragment.Brand;
                    return true;
                }
            }
        }

        result = null;
        return false;
    }
}
