using Shouldly;
using UaDetector.Abstractions.Enums;
using UaDetector.Abstractions.Models;
using UaDetector.Catalogs;
using UaDetector.Parsers;
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
            OsCatalog.OsNameMappings.ShouldContainKey(osName);
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

        foreach (var fixture in fixtures)
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
    }
}
