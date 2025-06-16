using System.Text.RegularExpressions;

namespace UaDetector.Models;

public sealed class RegexRule<T>
{
    public required Regex Regex { get; init; }
    public required T Result { get; init; }
}
