namespace UaDetector.SourceGenerator.Models;

public sealed record ClientRule
{
    public required string Regex { get; init; }
    public required string Name { get; init; }
    public string? Version { get; init; }
}
