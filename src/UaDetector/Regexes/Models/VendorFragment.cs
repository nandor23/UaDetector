using System.Text.RegularExpressions;

namespace UaDetector.Regexes.Models;

public sealed class VendorFragment
{
    public required string Brand { get; init; }
    public IEnumerable<Regex> Regexes { get; init; }
}
