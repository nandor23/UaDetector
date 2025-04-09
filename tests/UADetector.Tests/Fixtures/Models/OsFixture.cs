using UADetector.Models.Enums;

namespace UADetector.Tests.Fixtures.Models;

public class OsFixture
{
    public required string UserAgent { get; init; }
    public required OsInfo Os { get; init; }
    public Dictionary<string, string?>? Headers { get; init; }

    public class OsInfo
    {
        public required string Name { get; init; }
        public required OsCode Code { get; init; }
        public required string? Version { get; init; }
        public required string? Platform { get; init; }
        public required string? Family { get; init; }
    }
}
