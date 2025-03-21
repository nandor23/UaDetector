using UADetector.Models.Enums;

namespace UADetector.Results;

public class ClientInfo
{
    public ClientType Type { get; set; }
    public required string Name { get; init; }
    public required string? Version { get; init; }
}
