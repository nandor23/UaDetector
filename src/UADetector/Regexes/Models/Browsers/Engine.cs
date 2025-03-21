namespace UADetector.Regexes.Models.Browsers;

internal sealed class Engine
{
    public string? Default { get; init; }
    public Dictionary<string, string>? Versions { get; init; }
}
