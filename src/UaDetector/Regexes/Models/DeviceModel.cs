using System.Text.RegularExpressions;
using UaDetector.Models.Enums;

namespace UaDetector.Regexes.Models;

internal sealed class DeviceModel
{
    public required Regex Regex { get; init; }
    public required DeviceType? Type { get; init; }
    public required string? Brand { get; init; }

    public required string Name { get; init; }
}
