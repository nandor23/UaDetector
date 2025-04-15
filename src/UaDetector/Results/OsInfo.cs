using UaDetector.Models.Enums;

namespace UaDetector.Results;

public sealed class OsInfo
{
    public required string Name { get; init; }
    public required OsCode Code { get; init; }
    public required string? Version { get; init; }
    public required string? Platform { get; init; }
    public required string? Family { get; init; }
}
