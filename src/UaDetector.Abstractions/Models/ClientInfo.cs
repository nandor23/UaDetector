using UaDetector.Abstractions.Enums;

namespace UaDetector.Abstractions.Models;

public sealed record ClientInfo
{
    public required ClientType Type { get; init; }
    public required string Name { get; init; }
    public string? Version { get; init; }
}
