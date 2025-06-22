namespace UaDetector.SourceGenerator.Models;

internal sealed record EngineRule
{
    public required string Regex { get; init; }
    public required string Name { get; init; }
}
