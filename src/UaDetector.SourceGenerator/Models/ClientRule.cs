namespace UaDetector.SourceGenerator.Models;

internal sealed record ClientRule
{
    public required string Regex { get; init; }
    public required string Name { get; init; }
    public string? Version { get; init; }
}
