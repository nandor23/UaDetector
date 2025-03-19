namespace UADetector.Regexes.Models.Browsers;

public class Engine
{
    public string? Default { get; init; }
    public Dictionary<string, string>? Versions { get; init; }
}
