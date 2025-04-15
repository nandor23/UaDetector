using UaDetector.Results;

namespace UaDetector.Tests.Fixtures.Models;

public class DeviceFixture
{
    public required string UserAgent { get; init; }

    public required InternalDeviceInfo Device { get; init; }
}
