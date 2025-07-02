using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace UaDetector.YamlJsonConverter.Models;

public sealed class DeviceModelYaml
{
    public required Regex Regex { get; init; }

    [YamlMember(Alias = "device")]
    public required string? Type { get; init; }
    public required string? Brand { get; init; }

    [YamlMember(Alias = "model")]
    public required string Name { get; init; }
}
