using System.Text.RegularExpressions;

namespace UADetector.Regexes.Models;

internal sealed class Bot
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
    public required string? Category { get; init; }
    public required string? Url { get; init; }
    public required BotProducer? Producer { get; init; }
}
