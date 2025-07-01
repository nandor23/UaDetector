using Shouldly;
using UaDetector.Abstractions.Enums;
using UaDetector.Abstractions.Models;
using UaDetector.Parsers;
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
            BrowserParser.BrowserNameMapping.ShouldContainKey(browserName);
        }
    }

    [Test]
    public void BrowserCodeMapping_ShouldContainAllBrowserCodes()
    {
        foreach (BrowserCode browserCode in Enum.GetValues<BrowserCode>())
        {
            BrowserParser.BrowserCodeMapping.ShouldContainKey(browserCode);
        }
    }

    [Test]
    public void BrowserCodeMapping_ShouldContainUniqueValues()
    {
        BrowserParser.BrowserCodeMapping.Values.Length.ShouldBe(
            BrowserParser.BrowserCodeMapping.Values.Distinct().Count()
        );
    }

    [Test]
    public void CompactToFullNameMapping_ShouldContainKeyForAllUniqueNames()
    {
        var duplicateCompactNames = BrowserParser
            .BrowserCodeMapping.Values.Select(x => x.RemoveSpaces())
            .GroupBy(x => x)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        var browserNames = new List<string>();

        foreach (var name in BrowserParser.BrowserNameMapping.Keys)
        {
            var compactName = name.RemoveSpaces();

            if (!duplicateCompactNames.Contains(compactName))
            {
                browserNames.Add(compactName);
            }
        }

        foreach (var name in browserNames)
        {
            BrowserParser.CompactToFullNameMapping.ShouldContainKey(name);
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
}
