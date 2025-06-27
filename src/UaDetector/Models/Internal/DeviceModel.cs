using System.Text.RegularExpressions;
using UaDetector.Models.Enums;

namespace UaDetector.Models.Internal;

internal sealed class DeviceModel
{
    public required Regex Regex { get; init; }
    public DeviceType? Type { get; init; }
    public string? Brand { get; init; }
    public string? Name { get; init; }
}
