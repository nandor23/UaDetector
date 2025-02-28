namespace UADetector.Regexes.Models;

public class OperatingSystemRegex : IRegexPattern
{
    public required string Name { get; init; }
    public required string Regex { get; init; }
    public string? Version { get; init; }
    public List<OperatingSystemVersionRegex>? Versions { get; init; }
}
