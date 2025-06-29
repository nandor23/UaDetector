using System.Text.RegularExpressions;

namespace UaDetector.Abstractions.Models.Internal;

internal sealed class Engine
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
}
