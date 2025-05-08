using System.Text.RegularExpressions;

namespace UaDetector.YamlJsonConverter.Models;

public class BotYml
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
    public string? Category { get; init; }
    public string? Url { get; init; }
    public BotProducerYml? Producer { get; init; }
}
