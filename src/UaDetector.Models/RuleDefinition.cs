using System.Text.RegularExpressions;

namespace UaDetector.Models;

public sealed class RuleDefinition<T>
{
    public required Regex Regex { get; init; }
    public required T Result { get; init; }
}
