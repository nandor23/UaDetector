using UADetector.Results;

namespace UADetector.Tests.Fixtures.Models;

public class BotFixture
{
    public required string UserAgent { get; init; }
    public required BotInfo Bot { get; init; }
}
