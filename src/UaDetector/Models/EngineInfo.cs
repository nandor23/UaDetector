namespace UaDetector.Models;

public sealed class EngineInfo
{
    public string? Name { get; init; }
    public string? Version { get; init; }

    public override string ToString()
    {
        return string.Join(
            ", ",
            new[]
            {
                string.IsNullOrEmpty(Name) ? null : $"{nameof(Name)}: {Name}",
                string.IsNullOrEmpty(Version) ? null : $"{nameof(Version)}: {Version}",
            }.Where(x => !string.IsNullOrEmpty(x))
        );
    }
}
