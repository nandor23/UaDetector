using System.Text.RegularExpressions;

namespace UADetector.Regexes.Models;

public sealed class Os : IRegexPattern
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
    public string? Version { get; init; }
    public List<OsVersion>? Versions { get; init; }
}
