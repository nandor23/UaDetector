namespace UaDetector.Models.Browsers;

public class Browser
{
    public required string Name { get; init; }
    public string? Version { get; init; }
    public Engine? Engine { get; init; }
}
