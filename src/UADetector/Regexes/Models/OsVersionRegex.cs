using System.Text.RegularExpressions;

namespace UADetector.Regexes.Models;

public sealed class OsVersionRegex
{
    public required Regex Regex { get; init; }
    public required string Version { get; init; }
}
