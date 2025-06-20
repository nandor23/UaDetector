namespace UaDetector.SourceGenerator.Models;

public sealed record EngineRule
{
    public string? Default { get; init; }
    public Dictionary<string, string>? Versions { get; init; }
}
