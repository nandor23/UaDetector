using System.Text.RegularExpressions;
using UaDetector.Abstractions.Enums;

namespace UaDetector.Regexes.Models;

internal sealed class DeviceModel
{
    public required Regex Regex { get; init; }
    public DeviceType? Type { get; init; }
    public string? Brand { get; init; }
    public string? Name { get; init; }
}
