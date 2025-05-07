using System.Text.RegularExpressions;

namespace UaDetector.YamlJsonConverter.Models;

public sealed class ClientYaml
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
    public string? Version { get; init; }
}
