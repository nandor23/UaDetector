using System.Text.Encodings.Web;
using System.Text.Json;

using Shouldly;

using UADetector.Models.Enums;
using UADetector.Parsers;
using UADetector.Parsers.Devices;
using UADetector.Results;
using UADetector.Tests.Fixtures.Models;
using UADetector.Tests.Helpers;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UADetector.Tests.Tests;

public class UADetectorTests
{
    [Test]
    public void UADetector_Instantiation_ShouldNotThrowException()
    {
        Should.NotThrow(() => new UADetector());
    }

    [Test]
    public void UADetector_ShouldImplement_IUADetector()
    {
        var parser = new UADetector();
        parser.ShouldBeAssignableTo<IUADetector>();
    }

    [Test]
    public void TryParse_WhenUserAgentIsEmpty_ShouldReturnFalse()
    {
        var uaDetector = new UADetector();

        uaDetector.TryParse(string.Empty, out _).ShouldBeFalse();
    }

    [Test]
    public void TryParse_WhenUserAgentIsInvalid_ShouldReturnFalse()
    {
        var uaDetector = new UADetector();

        uaDetector.TryParse("12345", out _).ShouldBeFalse();
    }

    [Test]
    [MethodDataSource(nameof(FixtureFileNames))]
    public async Task TryParse_WithFixtureData_ShouldReturnExpectedUserAgentInfo(string fileName)
    {
        var fixturePath = Path.Combine("Fixtures", "Resources", "Collections", $"{fileName}.json");
        var fixtures = await FixtureLoader.LoadAsync<UserAgentFixture>(fixturePath);

        var uaDetector = new UADetector(new UADetectorOptions
        {
            VersionTruncation = VersionTruncation.None
        });

        foreach (var fixture in fixtures)
        {
            UserAgentInfo? result;

            if (fixture.Headers is null)
            {
                uaDetector.TryParse(fixture.UserAgent, out result).ShouldBeTrue();
            }
            else
            {
                uaDetector.TryParse(fixture.UserAgent, fixture.Headers, out result).ShouldBeTrue();
            }

            result?.Os.ShouldBeEquivalentTo(fixture.Os);
        }
    }

    public static IEnumerable<Func<string>> FixtureFileNames()
    {
        yield return () => "cameras";
        /*yield return () => "car_browsers";
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
        yield return () => "televisions_1";
        yield return () => "televisions_2";
        yield return () => "televisions_3";
        yield return () => "televisions_4";
        yield return () => "televisions_5";
        yield return () => "televisions";
        yield return () => "wearables";
        yield return () => "unknown";*/
    }
}
