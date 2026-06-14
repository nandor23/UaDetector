namespace UaDetector.Abstractions.Models;

public sealed record UserAgentInfo
{
    public OsInfo? Os { get; init; }
    public BrowserInfo? Browser { get; init; }
    public ClientInfo? Client { get; init; }
    public DeviceInfo? Device { get; init; }
    public BotInfo? Bot { get; init; }
}
