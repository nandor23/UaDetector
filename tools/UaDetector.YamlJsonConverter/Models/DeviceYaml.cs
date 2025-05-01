using System.Text.RegularExpressions;

using YamlDotNet.Serialization;

namespace UaDetector.YamlJsonConverter.Models;

public sealed class DeviceYaml
{
    public required Regex Regex { get; init; }

    [YamlMember(Alias = "device")]
    public required string Type { get; init; }
    public required string? Model { get; init; }

    [YamlMember(Alias = "models")]
    public required IEnumerable<DeviceModelYaml>? ModelVariants { get; init; }
}
