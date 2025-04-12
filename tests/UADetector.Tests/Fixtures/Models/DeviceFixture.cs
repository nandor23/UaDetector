using UADetector.Results;

namespace UADetector.Tests.Fixtures.Models;

public class DeviceFixture
{
    public required string UserAgent { get; init; }

    public required InternalDeviceInfo Device { get; init; }
}
