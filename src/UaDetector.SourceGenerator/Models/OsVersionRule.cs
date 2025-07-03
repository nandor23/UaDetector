namespace UaDetector.SourceGenerator.Models;

public sealed record OsVersionRule
{
    public required string Regex { get; init; }
    public required string Version { get; init; }
}
