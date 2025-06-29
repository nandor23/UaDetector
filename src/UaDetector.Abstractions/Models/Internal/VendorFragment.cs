using System.Text.RegularExpressions;

namespace UaDetector.Abstractions.Models.Internal;

internal sealed class VendorFragment
{
    public required string Brand { get; init; }
    public required IReadOnlyList<Regex> Regexes { get; init; }
}
