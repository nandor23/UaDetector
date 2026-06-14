using UaDetector.Abstractions.Enums;

namespace UaDetector.Abstractions.Models;

public sealed record DeviceInfo
{
    public DeviceType? Type { get; init; }
    public string? Model { get; init; }
    public BrandInfo? Brand { get; init; }
}
