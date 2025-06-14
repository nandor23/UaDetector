using System.Text.RegularExpressions;

namespace UaDetector.YamlJsonConverter.Models;

public sealed class OsYaml
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
    public string? Version { get; init; }
    public List<OsVersionYaml>? Versions { get; init; }
}

public sealed class OsVersionYaml
{
    public required Regex Regex { get; init; }
    public required string Version { get; init; }
}
