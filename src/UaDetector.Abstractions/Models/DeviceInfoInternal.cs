using UaDetector.Abstractions.Models.Enums;

namespace UaDetector.Abstractions.Models;

public sealed class DeviceInfoInternal
{
    public DeviceType? Type { get; init; }
    public string? Brand { get; init; }
    public string? Model { get; init; }
}
