using Shouldly;
using UaDetector.Abstractions.Enums;

namespace UaDetector.Abstractions.Tests.Enums;

public class BotCategoryTests
{
    [Test]
    public void BotCategory_ValuesShouldBeSequential()
    {
        var values = Enum.GetValues<BotCategory>().Cast<int>().ToList();

        for (int i = 0; i < values.Count; i++)
        {
            values[i].ShouldBe(i + 1);
        }
    }

    [Test]
    public void BotCategory_HasExpectedValues()
    {
        var expectedValues = new Dictionary<BotCategory, int>
        {
            { BotCategory.SearchBot, 1 },
            { BotCategory.SearchTools, 2 },
            { BotCategory.SecuritySearchBot, 3 },
            { BotCategory.Crawler, 4 },
            { BotCategory.Validator, 5 },
            { BotCategory.SecurityChecker, 6 },
            { BotCategory.FeedFetcher, 7 },
            { BotCategory.FeedReader, 8 },
            { BotCategory.FeedParser, 9 },
            { BotCategory.SiteMonitor, 10 },
            { BotCategory.NetworkMonitor, 11 },
            { BotCategory.ServiceAgent, 12 },
            { BotCategory.ServiceBot, 13 },
            { BotCategory.SocialMediaAgent, 14 },
            { BotCategory.ReadItLaterService, 15 },
            { BotCategory.Benchmark, 16 },
            { BotCategory.AiAgent, 17 },
            { BotCategory.AiAssistant, 18 },
            { BotCategory.AiDataScraper, 19 },
            { BotCategory.AiSearchCrawler, 20 },
        };

        expectedValues.Count.ShouldBe(Enum.GetValues<BotCategory>().Length);

        foreach (var botCategory in Enum.GetValues<BotCategory>())
        {
            ((int)botCategory).ShouldBe(expectedValues[botCategory]);
        }
    }
}
