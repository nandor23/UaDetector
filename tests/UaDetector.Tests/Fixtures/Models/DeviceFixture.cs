using UaDetector.Models;

namespace UaDetector.Tests.Fixtures.Models;

internal class DeviceFixture
{
    public required string UserAgent { get; init; }

    public required DeviceInfoInternal Device { get; init; }
}
