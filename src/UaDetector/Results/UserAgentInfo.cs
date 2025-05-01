namespace UaDetector.Results;

public sealed class UserAgentInfo
{
    public required OsInfo? Os { get; init; }
    public required BrowserInfo? Browser { get; init; }
    public required ClientInfo? Client { get; init; }
    public required DeviceInfo? Device { get; init; }
    public required BotInfo? Bot { get; init; }

    public override string ToString()
    {
        return string.Join(
            "\n",
            new[]
            {
                Os is null ? null : $"{nameof(Os)}: {Os}",
                Browser is null ? null : $"{nameof(Browser)}: {Browser}",
                Client is null ? null : $"{nameof(Client)}: {Client}",
                Device is null ? null : $"{nameof(Device)}: {Device}",
                Bot is null ? null : $"{nameof(Bot)}: {Bot}",
            }.Where(x => !string.IsNullOrEmpty(x))
        );
    }
}
