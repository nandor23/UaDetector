namespace UaDetector.Abstractions.Models;

public sealed record EngineInfo
{
    public string? Name { get; init; }
    public string? Version { get; init; }
}
