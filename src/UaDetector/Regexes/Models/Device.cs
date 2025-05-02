using System.Text.RegularExpressions;
using UaDetector.Models.Enums;

namespace UaDetector.Regexes.Models;

internal sealed class Device
{
    public required string Brand { get; init; }
    public required Regex Regex { get; init; }
    public DeviceType? Type { get; init; }
    public string? Model { get; init; }
    public IReadOnlyList<DeviceModel>? ModelVariants { get; init; }
}
