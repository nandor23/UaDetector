namespace UaDetector.Regexes.Models.Browsers;

internal sealed class Engine
{
    public string? Default { get; init; }
    public IReadOnlyDictionary<string, string>? Versions { get; init; }
}
