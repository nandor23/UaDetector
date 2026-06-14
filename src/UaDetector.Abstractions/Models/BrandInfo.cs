using UaDetector.Abstractions.Enums;

namespace UaDetector.Abstractions.Models;

public sealed record BrandInfo
{
    public required string Name { get; init; }
    public required BrandCode Code { get; init; }
}
