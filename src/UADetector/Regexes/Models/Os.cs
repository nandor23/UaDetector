using System.Text.RegularExpressions;

namespace UADetector.Regexes.Models;

internal sealed class Os
{
    public required Lazy<Regex> Regex { get; init; }
    public required string Name { get; init; }
    public string? Version { get; init; }
    public List<OsVersion>? Versions { get; init; }
}
