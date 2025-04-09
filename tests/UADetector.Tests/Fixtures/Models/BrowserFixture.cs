namespace UADetector.Tests.Fixtures.Models;

public class BrowserFixture
{
    public required string UserAgent { get; init; }
    public required BrowserInfo Browser { get; init; }
    public Dictionary<string, string?>? Headers { get; init; }

    public sealed class BrowserInfo
    {
        public required string Name { get; init; }
        public required string? Version { get; init; }
        public required string? Engine { get; init; }
        public required string? EngineVersion { get; init; }
        public required string? Family { get; init; }
    }
}
