using System.Text.RegularExpressions;

using YamlDotNet.Serialization;

namespace UADetector.Regexes.Models;

internal sealed class Device
{
    public required Lazy<Regex> Regex { get; init; }

    [YamlMember(Alias = "device")]
    public required string Category { get; init; }

    public string? Model { get; init; }

    [YamlMember(Alias = "models")]
    public IEnumerable<DeviceModel>? ModelVariants { get; init; }
}
