using UaDetector.Models.Enums;

namespace UaDetector.Results;

public sealed class ClientInfo
{
    public required ClientType Type { get; init; }
    public required string Name { get; init; }
    public string? Version { get; init; }

    public override string ToString()
    {
        return string.Join(
            ", ",
            new[]
            {
                $"{nameof(Type)}: {Type}",
                $"{nameof(Name)}: {Name}",
                string.IsNullOrEmpty(Version) ? null : $"{nameof(Version)}: {Version}",
            }.Where(x => !string.IsNullOrEmpty(x))
        );
    }
}
