using UADetector.Models.Enums;

namespace UADetector.Results;

public sealed class OsInfo
{
    public string Name { get; internal init; } = string.Empty;
    public OsCode Code { get; internal init; }
    public string? Version { get; internal init; }
    public string? Platform { get; internal init; }
    public string? Family { get; internal init; }
}
