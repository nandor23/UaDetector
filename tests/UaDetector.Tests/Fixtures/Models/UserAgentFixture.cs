using UaDetector.Abstractions.Models;

namespace UaDetector.Tests.Fixtures.Models;

public class UserAgentFixture
{
    public required string UserAgent { get; init; }
    public Dictionary<string, string?>? Headers { get; init; }
    public OsInfo? Os { get; init; }
    public BrowserInfo? Browser { get; init; }
    public ClientInfo? Client { get; init; }
    public DeviceInfo? Device { get; init; }
    public BotInfo? Bot { get; init; }
}
