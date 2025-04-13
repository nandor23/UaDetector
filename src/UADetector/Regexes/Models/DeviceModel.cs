using System.Text.RegularExpressions;

namespace UADetector.Regexes.Models;

internal sealed class DeviceModel
{
    public required Regex Regex { get; init; }
    public required string? Type { get; init; }
    public required string? Brand { get; init; }

    public required string Name { get; init; }
}
