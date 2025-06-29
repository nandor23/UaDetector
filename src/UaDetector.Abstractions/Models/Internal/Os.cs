using System.Text.RegularExpressions;

namespace UaDetector.Abstractions.Models.Internal;

internal sealed class Os
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
    public string? Version { get; init; }
    public IReadOnlyList<OsVersion>? Versions { get; init; }
}
