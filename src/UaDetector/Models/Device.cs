using System.Text.RegularExpressions;
using UaDetector.Abstractions.Enums;

namespace UaDetector.Models;

internal sealed class Device
{
    public required Regex Regex { get; init; }
    public required string Brand { get; init; }
    public DeviceType? Type { get; init; }
    public string? Model { get; init; }
    public IReadOnlyList<DeviceModel>? ModelVariants { get; init; }
}
