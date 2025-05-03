using UaDetector.Models.Enums;

namespace UaDetector.Results;

public sealed class DeviceInfoInternal
{
    public DeviceType? Type { get; init; }
    public string? Brand { get; init; }
    public string? Model { get; init; }
}
