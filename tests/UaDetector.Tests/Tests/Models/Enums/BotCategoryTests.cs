using Shouldly;
using UaDetector.Models.Enums;

namespace UaDetector.Tests.Tests.Models.Enums;

public class BotCategoryTests
{
    [Test]
    public void BotCategory_HasExpectedValues()
    {
        var expectedValues = new Dictionary<BotCategory, int>
        {
            { BotCategory.SearchBot, 0 },
            { BotCategory.SearchTools, 1 },
            { BotCategory.SecuritySearchBot, 2 },
            { BotCategory.Crawler, 3 },
            { BotCategory.Validator, 4 },
            { BotCategory.SecurityChecker, 5 },
            { BotCategory.FeedFetcher, 6 },
            { BotCategory.FeedReader, 7 },
            { BotCategory.FeedParser, 8 },
            { BotCategory.SiteMonitor, 9 },
            { BotCategory.NetworkMonitor, 10 },
            { BotCategory.ServiceAgent, 11 },
            { BotCategory.ServiceBot, 12 },
            { BotCategory.SocialMediaAgent, 13 },
            { BotCategory.ReadItLaterService, 14 },
            { BotCategory.Benchmark, 15 },
        };

        expectedValues.Count.ShouldBe(Enum.GetValues<BotCategory>().Length);
        
        foreach (var botCategory in Enum.GetValues<BotCategory>())
        {
            ((int)botCategory).ShouldBe(expectedValues[botCategory]);
        }
    }
}
