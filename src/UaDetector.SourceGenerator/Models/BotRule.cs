namespace UaDetector.SourceGenerator.Models;

public sealed record BotRule
{
    public required string Regex { get; init; }
    public required string Name { get; init; }
    public int? Category { get; init; }
    public string? Url { get; init; }
    public BotProducerRule? Producer { get; init; }
}
