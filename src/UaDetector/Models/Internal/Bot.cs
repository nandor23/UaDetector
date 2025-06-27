using System.Text.RegularExpressions;
using UaDetector.Models.Enums;

namespace UaDetector.Models.Internal;

internal sealed class Bot
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
    public BotCategory? Category { get; init; }
    public string? Url { get; init; }
    public BotProducer? Producer { get; init; }
}
