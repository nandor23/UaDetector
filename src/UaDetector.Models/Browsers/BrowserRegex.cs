namespace UaDetector.Models.Browsers;

public sealed record BrowserRegex : Browser
{
    public required string Regex { get; init; }
}
