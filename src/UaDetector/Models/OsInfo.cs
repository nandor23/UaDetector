using UaDetector.Models.Enums;

namespace UaDetector.Models;

public sealed class OsInfo
{
    public required string Name { get; init; }
    public required OsCode Code { get; init; }
    public string? Version { get; init; }
    public string? CpuArchitecture { get; init; }
    public string? Family { get; init; }

    public override string ToString()
    {
        return string.Join(
            ", ",
            new[]
            {
                $"{nameof(Name)}: {Name}",
                string.IsNullOrEmpty(Version) ? null : $"{nameof(Version)}: {Version}",
                string.IsNullOrEmpty(CpuArchitecture)
                    ? null
                    : $"{nameof(CpuArchitecture)}: {CpuArchitecture}",
                string.IsNullOrEmpty(Family) ? null : $"{nameof(Family)}: {Family}",
            }.Where(x => !string.IsNullOrEmpty(x))
        );
    }
}
