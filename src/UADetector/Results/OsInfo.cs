using UADetector.Models.Enums;

namespace UADetector.Results;

public class OsInfo
{
    public string? Name { get; internal init; }
    public OsCode? Code { get; internal init; }
    public string? Version { get; internal init; }
    public string? Platform { get; internal init; }
    public string? Family { get; internal init; }
}
