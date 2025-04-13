namespace UADetector.Results;

public sealed class UserAgentInfo
{
    public required bool IsBot { get; init; }
    public required OsInfo? Os { get; init; }
    public required BrowserInfo? Browser { get; init; }
    public required ClientInfo? Client { get; init; }
    public required DeviceInfo? Device { get; init; }
    public required BotInfo? Bot { get; init; }
}
