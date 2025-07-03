using UaDetector.SourceGenerator.Collections;

namespace UaDetector.SourceGenerator.Models;

public sealed record VendorFragmentRule
{
    public required string Brand { get; init; }
    public required EquatableReadOnlyList<string> Regexes { get; init; }
}
