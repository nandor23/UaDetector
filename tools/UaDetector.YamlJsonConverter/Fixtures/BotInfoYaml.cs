using UaDetector.Abstractions.Models;

namespace UaDetector.YamlJsonConverter.Fixtures;

public class BotInfoYaml
{
    public required string Name { get; init; }
    public required string? Category { get; init; }
    public required string? Url { get; init; }
    public required ProducerInfo? Producer { get; init; }
}
