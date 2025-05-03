using UaDetector.Results;

namespace UaDetector.Tests.Fixtures.Models;

public class BrowserFixture
{
    public required string UserAgent { get; init; }
    public Dictionary<string, string?>? Headers { get; init; }
    public required BrowserInfo Browser { get; init; }
}
