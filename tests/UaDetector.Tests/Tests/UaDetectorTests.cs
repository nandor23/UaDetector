using Shouldly;
using UaDetector.Abstractions.Enums;
using UaDetector.Abstractions.Models;
using UaDetector.Parsers;
using UaDetector.Tests.Fixtures.Models;
using UaDetector.Tests.Helpers;

namespace UaDetector.Tests.Tests;

public class UaDetectorTests
{
    [Test]
    public void UaDetector_Instantiation_ShouldNotThrowException()
    {
        Should.NotThrow(() => new UaDetector());
    }

    [Test]
    public void UaDetector_ShouldImplement_IUaDetector()
    {
        var uaDetector = new UaDetector();
        uaDetector.ShouldBeAssignableTo<IUaDetector>();
    }

    [Test]
    public void TryParse_WhenUserAgentIsEmpty_ShouldReturnFalse()
    {
        var uaDetector = new UaDetector();

        uaDetector.TryParse(string.Empty, out var result).ShouldBeFalse();
        result.ShouldBeNull();
    }

    [Test]
    public void TryParse_WhenUserAgentIsInvalid_ShouldReturnFalse()
    {
        var uaDetector = new UaDetector();

        uaDetector.TryParse("12345", out var result).ShouldBeFalse();
        result.ShouldBeNull();
    }

    [Test]
    [MethodDataSource(nameof(FixtureFileNames))]
    public async Task TryParse_ShouldReturnExpectedUserAgentInfo(string fixturePath)
    {
        var fixtures = await FixtureLoader.LoadAsync<UserAgentFixture>(fixturePath);
        var uaDetector = new UaDetector(
            new UaDetectorOptions { VersionTruncation = VersionTruncation.None }
        );

        foreach (var fixture in fixtures)
        {
            bool isParsed = fixture.Headers is null
                ? uaDetector.TryParse(fixture.UserAgent, out var result)
                : uaDetector.TryParse(fixture.UserAgent, fixture.Headers, out result);

            if (
                fixture.Os is null
                && fixture.Browser is null
                && fixture.Client is null
                && fixture.Device is null
                && fixture.Bot is null
            )
            {
                isParsed.ShouldBeFalse();
                result.ShouldBeNull();
            }
            else
            {
                isParsed.ShouldBeTrue();
                result.ShouldNotBeNull();
                result.Os.ShouldBeEquivalentTo(fixture.Os);
                result.Browser.ShouldBeEquivalentTo(fixture.Browser);
                result.Client.ShouldBeEquivalentTo(fixture.Client);
                result.Device.ShouldBeEquivalentTo(fixture.Device);
                result.Bot.ShouldBeEquivalentTo(fixture.Bot);
            }
        }
    }

    [Test]
    [MethodDataSource(nameof(FixtureFileNames))]
    public async Task TryParse_ShouldProduceConsistentResultsAcrossParsers(string fixturePath)
    {
        var fixtures = await FixtureLoader.LoadAsync<UserAgentFixture>(fixturePath);
        var uaDetectorOptions = new UaDetectorOptions
        {
            VersionTruncation = VersionTruncation.None,
        };
        var uaDetector = new UaDetector(uaDetectorOptions);
        var osParser = new OsParser(uaDetectorOptions);
        var browserParser = new BrowserParser(uaDetectorOptions);
        var clientParser = new ClientParser(uaDetectorOptions);

        foreach (var fixture in fixtures)
        {
            UserAgentInfo? userAgentInfo;
            OsInfo? osInfo;
            BrowserInfo? browserInfo;
            ClientInfo? clientInfo;

            if (fixture.Headers is null)
            {
                uaDetector.TryParse(fixture.UserAgent, out userAgentInfo);
                osParser.TryParse(fixture.UserAgent, out osInfo);
                browserParser.TryParse(fixture.UserAgent, out browserInfo);
                clientParser.TryParse(fixture.UserAgent, out clientInfo);
            }
            else
            {
                uaDetector.TryParse(fixture.UserAgent, fixture.Headers, out userAgentInfo);
                osParser.TryParse(fixture.UserAgent, fixture.Headers, out osInfo);
                browserParser.TryParse(fixture.UserAgent, fixture.Headers, out browserInfo);
                clientParser.TryParse(fixture.UserAgent, fixture.Headers, out clientInfo);
            }

            userAgentInfo?.Os.ShouldBeEquivalentTo(osInfo);
            userAgentInfo?.Browser.ShouldBeEquivalentTo(browserInfo);
            userAgentInfo?.Client.ShouldBeEquivalentTo(clientInfo);
        }
    }

    [Test]
    public void TryParse_WhenBotDetectionIsDisabled_ShouldReturnTrue()
    {
        var uaDetector = new UaDetector(new UaDetectorOptions { DisableBotDetection = true });
        var userAgent =
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1; 360Spider";

        uaDetector.TryParse(userAgent, out var result).ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Bot.ShouldBeNull();
    }

    [Test]
    public void TryParse_WhenBotDetectionIsEnabled_ShouldReturnTrue()
    {
        var uaDetector = new UaDetector();
        var userAgent =
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1; 360Spider";

        uaDetector.TryParse(userAgent, out var result).ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Bot.ShouldNotBeNull();
        result.Os.ShouldBeNull();
        result.Browser.ShouldBeNull();
        result.Client.ShouldBeNull();
        result.Device.ShouldBeNull();
    }

    public static IEnumerable<Func<string>> FixtureFileNames()
    {
        var fixturesPath = Path.Combine("Fixtures", "Resources", "Collections");

        foreach (var file in Directory.GetFiles(fixturesPath, "*.json"))
        {
            yield return () => file;
        }
    }
}
