namespace UaDetector.Models.Browsers;

public sealed record Engine
{
    public string? Default { get; init; }
    public Dictionary<string, string>? Versions { get; init; }
}
