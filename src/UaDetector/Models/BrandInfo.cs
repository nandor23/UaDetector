using UaDetector.Models.Enums;

namespace UaDetector.Models;

public sealed class BrandInfo
{
    public required string Name { get; init; }
    public required BrandCode Code { get; init; }
}
