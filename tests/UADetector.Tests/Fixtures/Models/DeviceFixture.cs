namespace UADetector.Tests.Fixtures.Models;

public class DeviceFixture
{
    public required string UserAgent { get; init; }

    public required DeviceInfo Device { get; init; }

    public class DeviceInfo
    {
        public required string Brand { get; init; }
        public required string? Model { get; init; }
    }
}
