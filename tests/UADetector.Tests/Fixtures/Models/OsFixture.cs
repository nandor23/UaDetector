using UADetector.Results;

namespace UADetector.Tests.Fixtures.Models;

internal sealed class OsFixture
{
    public required string UserAgent { get; init; }
    public required OsInfo Os { get; init; }
    public Dictionary<string,string?>? Headers { get; init; }
}
