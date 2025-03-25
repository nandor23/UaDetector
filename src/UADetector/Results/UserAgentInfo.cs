namespace UADetector.Results;

public sealed class UserAgentInfo
{
    public required OsInfo? Os { get; init; }
    public required BrowserInfo? Browser { get; init; }
    public required ClientInfo? Client { get; init; }
    public required DeviceInfo? Device { get; init; }
}
