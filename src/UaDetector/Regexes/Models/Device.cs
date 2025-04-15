using System.Text.RegularExpressions;

namespace UaDetector.Regexes.Models;

internal sealed class Device
{
    public required string Brand { get; init; }
    public required Regex Regex { get; init; }
    public required string Type { get; init; }
    public required string? Model { get; init; }
    public required IEnumerable<DeviceModel>? ModelVariants { get; init; }
}
