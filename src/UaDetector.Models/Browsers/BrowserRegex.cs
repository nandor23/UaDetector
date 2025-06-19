namespace UaDetector.Models.Browsers;

public sealed record BrowserRegex : Browser, IRegexPattern
{
    public required string Regex { get; init; }
}
