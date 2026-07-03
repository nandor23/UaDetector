using Shouldly;
using UaDetector.Abstractions;
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

    [Test]
    public void TryParse_WhenCacheProvided_ShouldStoreResultInCache()
    {
        const string userAgent =
            "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)";
        var cache = new RecordingCache();
        var parser = new BotParser(new BotParserOptions { Cache = cache });

        parser.TryParse(userAgent, out _);

        cache.SetKeys.ShouldContain(key => key.StartsWith("bot:"));
    }

    [Test]
    public void TryParse_WhenCalledTwiceWithSameUserAgent_ShouldServeSecondCallFromCache()
    {
        const string userAgent =
            "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)";
        var cache = new RecordingCache();
        var parser = new BotParser(new BotParserOptions { Cache = cache });

        parser.TryParse(userAgent, out _);
        int setsAfterFirstParse = cache.SetCount;

        parser.TryParse(userAgent, out _);

        cache.SetCount.ShouldBe(setsAfterFirstParse);
        cache.GetKeys.Count(key => key.StartsWith("bot:")).ShouldBe(2);
    }

    [Test]
    public void IsBot_WhenCacheProvided_ShouldStoreResultInCache()
    {
        var cache = new RecordingCache();
        var parser = new BotParser(new BotParserOptions { Cache = cache });

        parser.IsBot("360spider-image");

        cache.SetKeys.ShouldContain(key => key.StartsWith("isbot:"));
    }
}
