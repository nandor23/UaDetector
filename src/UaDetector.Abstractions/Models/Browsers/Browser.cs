namespace UaDetector.Abstractions.Models.Browsers;

public class Browser
{
    public required string Name { get; init; }
    public string? Version { get; init; }
    public BrowserEngine? Engine { get; init; }
}
