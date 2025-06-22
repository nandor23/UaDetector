namespace UaDetector.Models.Browsers;

public sealed class BrowserEngine
{
    public string? Default { get; init; }
    public IReadOnlyDictionary<string, string>? Versions { get; init; }
}
