using UaDetector.Abstractions.Enums;
using UaDetector.SourceGenerator.Collections;

namespace UaDetector.SourceGenerator.Models;

internal sealed record DeviceRule
{
    public required string Regex { get; init; }
    public required string Brand { get; init; }
    public DeviceType? Type { get; init; }
    public string? Model { get; init; }
    public EquatableReadOnlyList<DeviceModelRule>? ModelVariants { get; init; }
}
