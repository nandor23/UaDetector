using System.Text.RegularExpressions;

namespace UaDetector.Regexes.Models;

public sealed class RegexRule<T>
{
    public required Regex Regex { get; init; }
    public required T Result { get; init; }
}
