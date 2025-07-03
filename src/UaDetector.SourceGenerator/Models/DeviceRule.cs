using UaDetector.SourceGenerator.Collections;

namespace UaDetector.SourceGenerator.Models;

public sealed record DeviceRule
{
    public required string Regex { get; init; }
    public required string Brand { get; init; }
    public int? Type { get; init; }
    public string? Model { get; init; }
    public EquatableReadOnlyList<DeviceModelRule>? ModelVariants { get; init; }
}
