using UaDetector.Abstractions.Enums;

namespace UaDetector.Abstractions.Models;

public sealed record BrowserInfo
{
    public required string Name { get; init; }
    public required BrowserCode Code { get; init; }
    public string? Version { get; init; }
    public string? Family { get; init; }
    public EngineInfo? Engine { get; init; }
}
