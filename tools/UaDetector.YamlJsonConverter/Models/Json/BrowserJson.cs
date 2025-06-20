using UaDetector.Models.Browsers;

namespace UaDetector.YamlJsonConverter.Models.Json;

internal sealed class BrowserJson : Browser
{
    public required string Regex { get; init; }
}
