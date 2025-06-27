using System.Text.RegularExpressions;

namespace UaDetector.Models.Internal;

internal sealed class Engine
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
}
