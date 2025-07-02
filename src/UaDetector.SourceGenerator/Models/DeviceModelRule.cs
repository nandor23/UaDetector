namespace UaDetector.SourceGenerator.Models;

public sealed record DeviceModelRule
{
    public required string Regex { get; init; }
    public int? Type { get; init; }
    public string? Brand { get; init; }
    public string? Name { get; init; }
}
