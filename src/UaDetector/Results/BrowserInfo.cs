using UaDetector.Models.Enums;

namespace UaDetector.Results;

public sealed class BrowserInfo
{
    public required string Name { get; init; }
    public required BrowserCode Code { get; init; }
    public required string? Version { get; init; }
    public required string? Family { get; init; }
    public required EngineInfo? Engine { get; init; }

    public override string ToString()
    {
        return string.Join(
            ", ",
            new[]
            {
                $"{nameof(Name)}: {Name}",
                string.IsNullOrEmpty(Version) ? null : $"{nameof(Version)}: {Version}",
                string.IsNullOrEmpty(Family) ? null : $"{nameof(Family)}: {Family}",
                Engine is null ? null : $"{nameof(Engine)}: {Engine}",
            }.Where(x => !string.IsNullOrEmpty(x))
        );
    }
}
