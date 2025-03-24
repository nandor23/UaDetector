using UADetector.Models.Enums;

namespace UADetector.Results;

public sealed class BrandInfo
{
    public required string Name { get; init; }
    public required BrandCode Code { get; init; }
}
