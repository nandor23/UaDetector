using UaDetector.Models.Enums;

namespace UaDetector.Results;

public sealed class DeviceInfo
{
    public DeviceType? Type { get; init; }
    public string? Model { get; init; }
    public BrandInfo? Brand { get; init; }

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
