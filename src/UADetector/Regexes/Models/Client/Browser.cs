using System.Text.RegularExpressions;

namespace UADetector.Regexes.Models.Client;

internal sealed class Browser : IClient
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
    public string? Version { get; init; }
    public Engine? Engine { get; init; }
}
