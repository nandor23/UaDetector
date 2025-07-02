using System.Text.RegularExpressions;

namespace UaDetector.Models;

internal sealed class Engine
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
}
