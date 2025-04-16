using Shouldly;

using UaDetector.Parsers;
using UaDetector.Results;

using UaDetector.Tests.Fixtures.Models;
using UaDetector.Tests.Helpers;

namespace UaDetector.Tests.Tests.Parsers;

public class BotParserTests
{
    [Test]
    public void BotParser_Instantiation_ShouldNotThrowException()
    {
        Should.NotThrow(() => new BotParser());
    }

    [Test]
    public void BotParser_ShouldImplement_IBotParser()
    {
        var parser = new BotParser();
        parser.ShouldBeAssignableTo<IBotParser>();
    }

    [Test]
    public async Task TryParse_WithFixtureData_ShouldReturnExpectedBotInfo()
    {
        var fixturePath = Path.Combine("Fixtures", "Resources", "bots.json");
        var fixtures = (await FixtureLoader.LoadAsync<BotFixture>(fixturePath))
            .Select(e => new BotFixture
            {
                UserAgent = e.UserAgent,
                Bot = new BotInfo
                {
                    Name = e.Bot.Name,
                    Category = e.Bot.Category,
                    Url = e.Bot.Url,
                    Producer = e.Bot.Producer?.Name is null && e.Bot.Producer?.Url is null ? null : e.Bot.Producer
                }
            });

        var osParser = new BotParser();

        foreach (var fixture in fixtures)
        {
            osParser.TryParse(fixture.UserAgent, out var result).ShouldBeTrue();

            result.ShouldNotBeNull();
            result.Name.ShouldBe(fixture.Bot.Name);
            result.Category.ShouldBe(fixture.Bot.Category);
            result.Url.ShouldBe(fixture.Bot.Url);
            result.Producer.ShouldBeEquivalentTo(fixture.Bot.Producer);
        }
    }

    [Test]
    public void IsBot_ShouldReturnTrue()
    {
        BotParser.IsBot("360spider-image").ShouldBeTrue();
    }
}
