namespace UaDetector.YamlJsonConverter.Models;

public class BrowserYaml
{
    public required string Regex { get; init; }
    public required string Name { get; init; }
    public string? Version { get; init; }
    public EngineYml? Engine { get; init; }

    public sealed class EngineYml
    {
        public string? Default { get; init; }
        public Dictionary<string, string>? Versions { get; init; }
    }
}
