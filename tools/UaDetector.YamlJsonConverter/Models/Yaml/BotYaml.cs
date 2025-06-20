using System.Text.RegularExpressions;

namespace UaDetector.YamlJsonConverter.Models.Yaml;

public class BotYaml
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
    public string? Category { get; init; }
    public string? Url { get; init; }
    public BotProducerYaml? Producer { get; init; }
}
