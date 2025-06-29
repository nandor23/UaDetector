using UaDetector.Abstractions.Models;

namespace UaDetector.Tests.Fixtures.Models;

public class ClientFixture
{
    public required string UserAgent { get; init; }
    public required ClientInfo Client { get; init; }
}
