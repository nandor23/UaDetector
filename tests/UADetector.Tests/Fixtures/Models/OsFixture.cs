using UADetector.Results;

namespace UADetector.Tests.Fixtures.Models;

public class OsFixture
{
    public required string UserAgent { get; init; }
    public required Dictionary<string, string?>? Headers { get; init; }
    public required OsInfo Os { get; init; }
}
