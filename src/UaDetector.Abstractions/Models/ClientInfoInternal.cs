namespace UaDetector.Abstractions.Models;

public sealed class ClientInfoInternal
{
    public required string Name { get; init; }
    public string? Version { get; init; }
}
