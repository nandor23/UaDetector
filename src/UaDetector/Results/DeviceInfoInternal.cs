using UaDetector.Models.Enums;

namespace UaDetector.Results;

public sealed class DeviceInfoInternal
{
    public required DeviceType? Type { get; init; }
    public required string? Brand { get; init; }
    public required string? Model { get; init; }
}
