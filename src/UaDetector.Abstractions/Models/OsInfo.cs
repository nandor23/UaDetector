using UaDetector.Abstractions.Enums;

namespace UaDetector.Abstractions.Models;

public sealed record OsInfo
{
    public required string Name { get; init; }
    public required OsCode Code { get; init; }
    public string? Version { get; init; }
    public string? CpuArchitecture { get; init; }
    public string? Family { get; init; }
}
