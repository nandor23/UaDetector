using UADetector.Models.Enums;

namespace UADetector.Results.Client;

public sealed class BrowserInfo : IClientInfo
{
    public string? Name { get; init; }
    public string? Version { get; init; }
    public BrowserCode? Code { get; init; }
    public string? Engine { get; init; }
    public string? EngineVersion { get; init; }
}
