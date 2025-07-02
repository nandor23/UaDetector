namespace UaDetector.YamlJsonConverter.Fixtures;

public sealed class UserAgentFixtureYaml
{
    public required string UserAgent { get; init; }
    public required Dictionary<string, string?>? Headers { get; init; }
    public required OsInfoYaml? Os { get; init; }
    public required ClientInfoYaml? Client { get; init; }
    public required DeviceInfoYaml? Device { get; init; }
    public required BotInfoYaml? Bot { get; init; }
    public required string? OsFamily { get; init; }
    public required string? BrowserFamily { get; init; }
}
