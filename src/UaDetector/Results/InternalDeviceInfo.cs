using UaDetector.Models.Enums;

namespace UaDetector.Results;

public class InternalDeviceInfo
{
    public required DeviceType? Type { get; init; }
    public required string? Brand { get; init; }
    public required string? Model { get; init; }
}
