using System.Text.RegularExpressions;

namespace UaDetector.Abstractions.Models;

public sealed class VendorFragment
{
    public required string Brand { get; init; }
    public required IReadOnlyList<Regex> Regexes { get; init; }
}
