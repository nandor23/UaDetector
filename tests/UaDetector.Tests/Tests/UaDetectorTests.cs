using Shouldly;

using UaDetector.Models;
using UaDetector.Models.Enums;
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
    public async Task TryParse_ShouldReturnExpectedUserAgentInfo(string fileName)
    {
        var fixturePath = Path.Combine("Fixtures", "Resources", "Collections", $"{fileName}.json");
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
    public async Task TryParse_ShouldProduceConsistentResultsAcrossParsers(string fileName)
    {
        var fixturePath = Path.Combine("Fixtures", "Resources", "Collections", $"{fileName}.json");
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
        yield return () => "cameras";
        yield return () => "car_browsers";
        yield return () => "client_hints";
        yield return () => "client_hints_apps";
        yield return () => "consoles";
        yield return () => "desktops";
        yield return () => "desktops_1";
        yield return () => "feature_phones";
        yield return () => "feed_readers";
        yield return () => "media_players";
        yield return () => "mobile_apps";
        yield return () => "peripherals";
        yield return () => "phablets";
        yield return () => "phablets_1";
        yield return () => "podcasting_apps";
        yield return () => "portable_media_players";
        yield return () => "smart_displays";
        yield return () => "smart_speakers";
        yield return () => "smartphones";
        yield return () => "smartphones_1";
        yield return () => "smartphones_2";
        yield return () => "smartphones_3";
        yield return () => "smartphones_4";
        yield return () => "smartphones_5";
        yield return () => "smartphones_6";
        yield return () => "smartphones_7";
        yield return () => "smartphones_8";
        yield return () => "smartphones_9";
        yield return () => "smartphones_10";
        yield return () => "smartphones_11";
        yield return () => "smartphones_12";
        yield return () => "smartphones_13";
        yield return () => "smartphones_14";
        yield return () => "smartphones_15";
        yield return () => "smartphones_16";
        yield return () => "smartphones_17";
        yield return () => "smartphones_18";
        yield return () => "smartphones_19";
        yield return () => "smartphones_20";
        yield return () => "smartphones_21";
        yield return () => "smartphones_22";
        yield return () => "smartphones_23";
        yield return () => "smartphones_24";
        yield return () => "smartphones_25";
        yield return () => "smartphones_26";
        yield return () => "smartphones_27";
        yield return () => "smartphones_28";
        yield return () => "smartphones_29";
        yield return () => "smartphones_30";
        yield return () => "smartphones_31";
        yield return () => "smartphones_32";
        yield return () => "smartphones_33";
        yield return () => "smartphones_34";
        yield return () => "smartphones_35";
        yield return () => "smartphones_36";
        yield return () => "smartphones_37";
        yield return () => "smartphones_38";
        yield return () => "smartphones_39";
        yield return () => "smartphones_40";
        yield return () => "smartphones_41";
        yield return () => "tablets";
        yield return () => "tablets_1";
        yield return () => "tablets_2";
        yield return () => "tablets_3";
        yield return () => "tablets_4";
        yield return () => "tablets_5";
        yield return () => "tablets_6";
        yield return () => "tablets_7";
        yield return () => "tablets_8";
        yield return () => "tablets_9";
        yield return () => "tablets_10";
        yield return () => "tablets_11";
        yield return () => "tablets_12";
        yield return () => "televisions";
        yield return () => "televisions_1";
        yield return () => "televisions_2";
        yield return () => "televisions_3";
        yield return () => "televisions_4";
        yield return () => "televisions_5";
        yield return () => "wearables";
        yield return () => "unknown";
    }
}
