using UADetector.Models.Enums;

namespace UADetector.Results;

public sealed class DeviceInfo
{
    public required DeviceType? Type { get; init; }
    public required BrandInfo? Brand { get; init; }
    public required string? Model { get; init; }
}
