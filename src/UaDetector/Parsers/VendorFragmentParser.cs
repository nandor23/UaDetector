using System.Diagnostics.CodeAnalysis;
using UaDetector.Regexes.Models;
using UaDetector.Utils;

namespace UaDetector.Parsers;

internal sealed class VendorFragmentParser
{
    private const string ResourceName = "Regexes.Resources.vendor_fragments.json";
    public static readonly IReadOnlyList<VendorFragment> VendorFragments;

    static VendorFragmentParser()
    {
        VendorFragments = RegexLoader.LoadRegexes<VendorFragment>(ResourceName, "[^a-z0-9]+");
    }

    public bool TryParseBrand(string userAgent, [NotNullWhen(true)] out string? result)
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
