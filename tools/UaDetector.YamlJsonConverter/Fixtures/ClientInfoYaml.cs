namespace UaDetector.YamlJsonConverter.Fixtures;

public sealed class ClientInfoYaml
{
    public required string Type { get; init; }
    public required string Name { get; init; }
    public required string? Version { get; init; }
    public required string? Family { get; init; }
    public required string? Engine { get; init; }
    public required string? EngineVersion { get; init; }
}
