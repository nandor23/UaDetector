using UaDetector.Abstractions.Enums;

namespace UaDetector.Abstractions.Models;

public sealed record BotInfo
{
    public required string Name { get; init; }
    public BotCategory? Category { get; init; }
    public string? Url { get; init; }
    public ProducerInfo? Producer { get; init; }
}
