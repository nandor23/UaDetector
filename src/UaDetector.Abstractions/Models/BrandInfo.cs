using UaDetector.Abstractions.Models.Enums;

namespace UaDetector.Abstractions.Models;

public sealed class BrandInfo
{
    public required string Name { get; init; }
    public required BrandCode Code { get; init; }
}
