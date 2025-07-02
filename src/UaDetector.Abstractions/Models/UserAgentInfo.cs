namespace UaDetector.Abstractions.Models;

public sealed class UserAgentInfo
{
    public OsInfo? Os { get; init; }
    public BrowserInfo? Browser { get; init; }
    public ClientInfo? Client { get; init; }
    public DeviceInfo? Device { get; init; }
    public BotInfo? Bot { get; init; }

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
