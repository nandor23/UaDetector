using System.Text.RegularExpressions;

using UaDetector.Models.Enums;

namespace UaDetector.Regexes.Models;

internal sealed class Bot
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
    public required BotCategory? Category { get; init; }
    public required string? Url { get; init; }
    public required BotProducer? Producer { get; init; }
}
