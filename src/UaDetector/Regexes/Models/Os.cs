using System.Text.RegularExpressions;

namespace UaDetector.Regexes.Models;

internal sealed class Os
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
    public string? Version { get; init; }
    public List<OsVersion>? Versions { get; init; }
}
