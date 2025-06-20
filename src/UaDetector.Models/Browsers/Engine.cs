namespace UaDetector.Models.Browsers;

public sealed record Engine
{
    public string? Default { get; init; }
    public IReadOnlyDictionary<string, string>? Versions { get; init; }
}
