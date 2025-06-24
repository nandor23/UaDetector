namespace UaDetector.YamlJsonConverter.Models;

public sealed class ClientYaml
{
    public required string Regex { get; init; }
    public required string Name { get; init; }
    public string? Version { get; init; }
}
