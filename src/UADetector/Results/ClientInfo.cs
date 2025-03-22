using UADetector.Models.Enums;

namespace UADetector.Results;

public sealed class ClientInfo
{
    public required ClientType Type { get; init; }
    public required string Name { get; init; }
    public required string? Version { get; init; }
}
