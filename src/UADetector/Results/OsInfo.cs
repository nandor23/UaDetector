using UADetector.Models;

namespace UADetector.Results;

public class OsInfo
{
    public string? Name { get; internal set; }
    public OsCode? Code { get; internal set; }
    public string? Version { get; internal set; }
}
