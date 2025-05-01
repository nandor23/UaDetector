using UaDetector.Models.Enums;

namespace UaDetector.Results;

public sealed class DeviceInfo
{
    public required DeviceType? Type { get; init; }
    public required string? Model { get; init; }
    public required BrandInfo? Brand { get; init; }

    public override string ToString()
    {
        return string.Join(
            ", ",
            new[]
            {
                Type is null ? null : $"{nameof(Type)}: {Type}",
                string.IsNullOrEmpty(Model) ? null : $"{nameof(Model)}: {Model}",
                string.IsNullOrEmpty(Brand?.Name) ? null : $"{nameof(Brand)}: {Brand?.Name}",
            }.Where(x => !string.IsNullOrEmpty(x))
        );
    }
}
