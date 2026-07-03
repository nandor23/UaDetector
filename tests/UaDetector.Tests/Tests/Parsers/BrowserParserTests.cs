using Shouldly;
using UaDetector.Abstractions;
using UaDetector.Abstractions.Enums;
using UaDetector.Abstractions.Models;
using UaDetector.Parsers;
using UaDetector.Registries;
using UaDetector.Tests.Fixtures.Models;
using UaDetector.Tests.Helpers;
using UaDetector.Utilities;

namespace UaDetector.Tests.Tests.Parsers;

public class BrowserParserTests
{
    [Test]
    public void BrowserParser_Instantiation_ShouldNotThrowException()
    {
        Should.NotThrow(() => new BrowserParser());
    }

    [Test]
    public void BrowserParser_ShouldImplement_IBrowserParser()
    {
        var parser = new BrowserParser();
        parser.ShouldBeAssignableTo<IBrowserParser>();
    }

    [Test]
    public void Browsers_ShouldContainKeysForAllBrowserNames()
    {
        var browserNames = BrowserParser.Browsers.Select(rule => rule.Name);

        foreach (var browserName in browserNames)
        {
            BrowserRegistry.BrowserNameMappings.ShouldContainKey(browserName);
        }
    }

    [Test]
    public void CompactToFullNameMapping_ShouldContainKeyForAllUniqueNames()
    {
        var duplicateCompactNames = BrowserRegistry
            .BrowserCodeMappings.Values.Select(x => x.RemoveSpaces())
            .GroupBy(x => x)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        var browserNames = new List<string>();

        foreach (var name in BrowserRegistry.BrowserNameMappings.Keys)
        {
            var compactName = name.RemoveSpaces();

            if (!duplicateCompactNames.Contains(compactName))
            {
                browserNames.Add(compactName);
            }
        }

        foreach (var name in browserNames)
        {
            BrowserParser.CompactToFullNameMappings.ShouldContainKey(name);
        }
    }

    [Test]
    public async Task TryParse_WithFixtureData_ShouldReturnExpectedBrowserInfo()
    {
        var fixturePath = Path.Combine("Fixtures", "Resources", "browsers.json");
        var fixtures = await FixtureLoader.LoadAsync<BrowserFixture>(fixturePath);
        var parser = new BrowserParser(
            new UaDetectorOptions { VersionTruncation = VersionTruncation.None }
        );

        foreach (var fixture in fixtures)
        {
            BrowserInfo? result;

            if (fixture.Headers is null)
            {
                parser.TryParse(fixture.UserAgent, out result).ShouldBeTrue();
            }
            else
            {
                parser.TryParse(fixture.UserAgent, fixture.Headers, out result).ShouldBeTrue();
            }

            result.ShouldNotBeNull();
            result.Name.ShouldBe(fixture.Browser.Name);
            result.Code.ShouldBe(fixture.Browser.Code);
            result.Version.ShouldBe(fixture.Browser.Version);
            result.Family.ShouldBe(fixture.Browser.Family);
        }
    }

    [Test]
    public void TryParse_WhenCacheProvided_ShouldStoreResultInCache()
    {
        const string userAgent =
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1";
        var cache = new RecordingCache();
        var parser = new BrowserParser(new UaDetectorOptions { Cache = cache });

        parser.TryParse(userAgent, out _).ShouldBeTrue();

        cache.SetKeys.ShouldContain(key => key.StartsWith("browser:"));
    }

    [Test]
    public void TryParse_WhenCalledTwiceWithSameUserAgent_ShouldServeSecondCallFromCache()
    {
        const string userAgent =
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1";
        var cache = new RecordingCache();
        var parser = new BrowserParser(new UaDetectorOptions { Cache = cache });

        parser.TryParse(userAgent, out _).ShouldBeTrue();
        int setsAfterFirstParse = cache.SetCount;

        parser.TryParse(userAgent, out _).ShouldBeTrue();

        cache.SetCount.ShouldBe(setsAfterFirstParse);
        cache.GetKeys.Count(key => key.StartsWith("browser:")).ShouldBe(2);
    }
}
