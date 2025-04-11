namespace UADetector.Tests.Fixtures.Models;

public class ClientFixture
{
    public required string UserAgent { get; init; }
    public required ClientInfo Client { get; init; }

    public class ClientInfo
    {
        public required string Name { get; init; }
        public required string? Version { get; init; }
    }
}
