using UaDetector.Abstractions.Enums;

namespace UaDetector.Results;

public sealed class BrandInfo
{
    public required string Name { get; init; }
    public required BrandCode Code { get; init; }
}
