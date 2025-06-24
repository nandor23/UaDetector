using System.Text.RegularExpressions;

namespace UaDetector.Abstractions.Models.Browsers;

public sealed class Engine
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
}
