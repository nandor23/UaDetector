using UaDetector.Models.Enums;

namespace UaDetector.Results;

public sealed class BotInfo
{
    public required string Name { get; init; }
    public required BotCategory? Category { get; init; }
    public required string? Url { get; init; }
    public required ProducerInfo? Producer { get; init; }
}
