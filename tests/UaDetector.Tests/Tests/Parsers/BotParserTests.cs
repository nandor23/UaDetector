using Shouldly;
using UaDetector.Parsers;
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
        var fixtures = (await FixtureLoader.LoadAsync<BotFixture>(fixturePath));
        var parser = new BotParser();

        foreach (var fixture in fixtures)
        {
            parser.TryParse(fixture.UserAgent, out var result).ShouldBeTrue();

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
        var parser = new BotParser();
        parser.IsBot("360spider-image").ShouldBeTrue();
    }
}
