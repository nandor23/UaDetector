using UADetector.Models.Enums;
using UADetector.Results;

namespace UADetector.Tests.Fixtures.Models;

public class UserAgentFixture
{
    public required string UserAgent { get; init; }
    public required Dictionary<string, string?>? Headers { get; init; }
    public required OsInfo? Os { get; init; }
    public required BrowserInfo? Browser { get; init; }
    public required DeviceInfo? Device { get; init; }


    public class DeviceInfo
    {
        public required DeviceType? Type { get; init; }
        public required string Brand { get; init; }
        public required string? Model { get; init; }
    }

    public sealed class BotInfo
    {
        public required string Name { get; init; }
        public required string? Category { get; init; }
        public required string? Url { get; init; }
    }
}
