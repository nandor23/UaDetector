using UaDetector.Abstractions.Enums;

namespace UaDetector.Abstractions.Models;

public sealed class BrandInfo
{
    public required string Name { get; init; }
    public required BrandCode Code { get; init; }
    
    public override string ToString()
    {
        return $"{nameof(Name)}: {Name}, {nameof(Code)}: {Code}";
    }
}
