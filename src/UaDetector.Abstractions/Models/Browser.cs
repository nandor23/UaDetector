using System.Text.RegularExpressions;

namespace UaDetector.Abstractions.Models;

public sealed class Browser
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
    public string? Version { get; init; }
    public BrowserEngine? Engine { get; init; }
}
