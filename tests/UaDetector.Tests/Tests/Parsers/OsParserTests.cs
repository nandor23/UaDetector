using Shouldly;
using UaDetector.Abstractions;
using UaDetector.Abstractions.Enums;
using UaDetector.Abstractions.Models;
using UaDetector.Parsers;
using UaDetector.Registries;
using UaDetector.Tests.Fixtures.Models;
using UaDetector.Tests.Helpers;

namespace UaDetector.Tests.Tests.Parsers;

public class OsParserTests
{
    [Test]
    public void OsParser_Instantiation_ShouldNotThrowException()
    {
        Should.NotThrow(() => new OsParser());
    }

    [Test]
    public void OsParser_ShouldImplement_IOsParser()
    {
        var parser = new OsParser();
        parser.ShouldBeAssignableTo<IOsParser>();
    }

    [Test]
    public void OperatingSystems_ShouldContainKeyForAllOsNames()
    {
        var osNames = OsParser.OperatingSystems.Where(os => os.Name != "$1").Select(os => os.Name);

        foreach (var osName in osNames)
        {
            OsRegistry.TryGetOsCode(osName, out var osCode).ShouldBeTrue();
            osCode.ShouldNotBeNull();
        }
    }

    [Test]
    public void OsFamilyMapping_ShouldContainKeyForAllOsCodes()
    {
        foreach (OsCode osCode in Enum.GetValues<OsCode>())
        {
            bool contains = false;

            foreach (var osFamily in OsParser.OsFamilyMappings)
            {
                if (osFamily.Value.Contains(osCode))
                {
                    contains = true;
                    break;
                }
            }

            contains.ShouldBeTrue();
        }
    }

    [Test]
    public async Task TryParse_WithFixtureData_ShouldReturnExpectedOsInfo()
    {
        var fixturePath = Path.Combine("Fixtures", "Resources", "operating_systems.json");
        var fixtures = await FixtureLoader.LoadAsync<OsFixture>(fixturePath);
        var parser = new OsParser(
            new UaDetectorOptions { VersionTruncation = VersionTruncation.None }
        );

        FixtureAssert.ForEach(
            fixturePath,
            fixtures,
            fixture =>
            {
                OsInfo? result;

                if (fixture.Headers is null)
                {
                    parser.TryParse(fixture.UserAgent, out result).ShouldBeTrue();
                }
                else
                {
                    parser.TryParse(fixture.UserAgent, fixture.Headers, out result).ShouldBeTrue();
                }

                result.ShouldNotBeNull();
                result.Name.ShouldBe(fixture.Os.Name);
                result.Code.ShouldBe(fixture.Os.Code);
                result.Version.ShouldBe(fixture.Os.Version);
                result.CpuArchitecture.ShouldBe(fixture.Os.CpuArchitecture);
                result.Family.ShouldBe(fixture.Os.Family);
            }
        );
    }

    [Test]
    public void TryParse_WhenCacheProvided_ShouldStoreResultInCache()
    {
        const string userAgent =
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1";
        var cache = new RecordingCache();
        var parser = new OsParser(new UaDetectorOptions { Cache = cache });

        parser.TryParse(userAgent, out _).ShouldBeTrue();

        cache.SetKeys.ShouldContain(key => key.StartsWith("os:"));
    }

    [Test]
    public void TryParse_WhenCalledTwiceWithSameUserAgent_ShouldServeSecondCallFromCache()
    {
        const string userAgent =
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1";
        var cache = new RecordingCache();
        var parser = new OsParser(new UaDetectorOptions { Cache = cache });

        parser.TryParse(userAgent, out _).ShouldBeTrue();
        int setsAfterFirstParse = cache.SetCount;

        parser.TryParse(userAgent, out _).ShouldBeTrue();

        cache.SetCount.ShouldBe(setsAfterFirstParse);
        cache.GetKeys.Count(key => key.StartsWith("os:")).ShouldBe(2);
    }
}
