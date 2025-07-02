using UaDetector.Abstractions.Models;

namespace UaDetector.Tests.Fixtures.Models;

public class DeviceFixture
{
    public required string UserAgent { get; init; }

    public required DeviceInfoInternal Device { get; init; }
}
