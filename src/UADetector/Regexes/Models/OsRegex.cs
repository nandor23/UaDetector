namespace UADetector.Regexes.Models;

public class OsRegex : IRegexPattern
{
    public required string Name { get; init; }
    public required string Regex { get; init; }
    public string? Version { get; init; }
    public List<OsVersionRegex>? Versions { get; init; }
}
