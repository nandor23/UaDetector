using UaDetector.Abstractions.Enums;

namespace UaDetector.SourceGenerator.Models;

internal sealed record DeviceModelRule
{
    public required string Regex { get; init; }
    public DeviceType? Type { get; init; }
    public string? Brand { get; init; }
    public string? Name { get; init; }
}
