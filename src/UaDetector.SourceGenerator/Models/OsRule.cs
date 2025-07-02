using UaDetector.SourceGenerator.Collections;

namespace UaDetector.SourceGenerator.Models;

public sealed record OsRule
{
    public required string Regex { get; init; }
    public required string Name { get; init; }
    public string? Version { get; init; }
    public EquatableReadOnlyList<OsVersionRule>? Versions { get; init; }
}
