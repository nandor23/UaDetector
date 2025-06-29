using UaDetector.Abstractions.Models;

namespace UaDetector.Tests.Fixtures.Models;

public class OsFixture
{
    public required string UserAgent { get; init; }
    public Dictionary<string, string?>? Headers { get; init; }
    public required OsInfo Os { get; init; }
}
