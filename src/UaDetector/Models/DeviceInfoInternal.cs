using UaDetector.Abstractions.Enums;

namespace UaDetector.Models;

internal sealed class DeviceInfoInternal
{
    public DeviceType? Type { get; init; }
    public string? Brand { get; init; }
    public string? Model { get; init; }
}
