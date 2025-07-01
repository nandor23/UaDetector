namespace UaDetector.Models;

internal sealed class BrowserEngine
{
    public string? Default { get; init; }
    public IReadOnlyDictionary<string, string>? Versions { get; init; }
}
