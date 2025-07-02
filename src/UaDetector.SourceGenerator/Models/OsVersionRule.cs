namespace UaDetector.SourceGenerator.Models;

internal sealed record OsVersionRule
{
    public required string Regex { get; init; }
    public required string Version { get; init; }
}
