using System.Text.RegularExpressions;

namespace UaDetector.Regexes.Models;

internal sealed class Bot
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
    public required string? Category { get; init; }
    public required string? Url { get; init; }
    public required BotProducer? Producer { get; init; }
}
