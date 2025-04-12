using UADetector.Results;

namespace UADetector.Tests.Fixtures.Models;

public class UserAgentFixture
{
    public required string UserAgent { get; init; }
    public required Dictionary<string, string?>? Headers { get; init; }
    public required OsInfo? Os { get; init; }
    public required BrowserInfo? Browser { get; init; }
    public required ClientInfo? Client { get; init; }
    public required DeviceInfo? Device { get; init; }
    public required BotInfo? Bot { get; init; }
}
