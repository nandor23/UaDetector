using UaDetector.Models.Browsers;

namespace UaDetector.YamlJsonConverter.Models.Json;

internal record BrowserJson : Browser
{
    public required string Regex { get; init; }
}
