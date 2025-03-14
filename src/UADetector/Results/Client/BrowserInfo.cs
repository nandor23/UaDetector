using UADetector.Models.Enums;

namespace UADetector.Results.Client;

public sealed class BrowserInfo : IClientInfo
{
    public required string Name { get; init; }
    public required string? Version { get; init; }
    public required BrowserCode Code { get; init; }
    public required string? Family { get; init; }
    public required string? Engine { get; init; }
    public required string? EngineVersion { get; init; }
}
