using System.Text.RegularExpressions;
using UaDetector.Regexes.Models.Browsers;

namespace UaDetector.YamlJsonConverter.Models;

public class BrowserYaml
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
    public string? Version { get; init; }
    public EngineYml? Engine { get; init; }

    public sealed class EngineYml
    {
        public string? Default { get; init; }
        public Dictionary<string, string>? Versions { get; init; }
    }
}
