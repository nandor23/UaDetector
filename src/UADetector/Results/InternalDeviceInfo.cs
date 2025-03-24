using UADetector.Models.Enums;

namespace UADetector.Results;

public class InternalDeviceInfo
{
    public required DeviceType? Type { get; init; }
    public required string? Brand { get; init; }
    public required string? Model { get; init; }
}
