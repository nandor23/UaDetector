using System.Text.RegularExpressions;

namespace UADetector.Regexes.Models;

internal sealed class OsVersion
{
    public required Lazy<Regex> Regex { get; init; }
    public required string Version { get; init; }
}
