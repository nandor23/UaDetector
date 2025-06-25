using UaDetector.Abstractions.Enums;

namespace UaDetector.SourceGenerator.Models;

internal sealed record BotRule
{
    public required string Regex { get; init; }
    public required string Name { get; init; }
    public BotCategory? Category { get; init; }
    public string? Url { get; init; }
    public BotProducerRule? Producer { get; init; }
}
