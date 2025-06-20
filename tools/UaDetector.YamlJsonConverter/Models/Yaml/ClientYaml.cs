using System.Text.RegularExpressions;

namespace UaDetector.YamlJsonConverter.Models.Yaml;

public sealed class ClientYaml
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
    public string? Version { get; init; }
}
