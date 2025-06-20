using System.Text.RegularExpressions;

namespace UaDetector.Models.Browsers;

public sealed record BrowserEngine
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
}
