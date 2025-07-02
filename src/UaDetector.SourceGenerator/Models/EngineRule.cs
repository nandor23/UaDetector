namespace UaDetector.SourceGenerator.Models;

public sealed record EngineRule
{
    public required string Regex { get; init; }
    public required string Name { get; init; }
}
