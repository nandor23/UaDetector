using UADetector.Models;

namespace UADetector.Results;

public class OsInfo
{
    public required string Name { get; init; }
    public OsCode Code { get; init; }
}
