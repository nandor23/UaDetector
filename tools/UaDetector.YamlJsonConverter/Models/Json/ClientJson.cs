using UaDetector.Models;

namespace UaDetector.YamlJsonConverter.Models.Json;

internal sealed class ClientJson : Client
{
    public required string Regex { get; init; }
}
