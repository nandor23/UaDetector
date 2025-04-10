using Shouldly;

using UADetector.Models.Constants;
using UADetector.Models.Enums;
using UADetector.Parsers;
using UADetector.Results;
using UADetector.Tests.Fixtures.Models;
using UADetector.Tests.Helpers;

namespace UADetector.Tests.Tests.Parsers;

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
    public void OperatingSystems_ShouldHaveNameMapping()
    {
        var osNames = OsParser.OperatingSystems
            .Where(os => os.Name != "$1")
            .Select(os => os.Name);

        foreach (var osName in osNames)
        {
            OsParser.OsNameMapping.ShouldContainKey(osName);
        }
    }

    [Test]
    public void OsNameMappings_ShouldMatchParsedOsNames()
    {
        var ignoredNames = new List<string>
        {
            OsNames.BackTrack,
            OsNames.Kubuntu,
            OsNames.Lubuntu,
            OsNames.Xubuntu,
            OsNames.Gentoo,
            OsNames.ChromiumOs,
            OsNames.Slackware,
            OsNames.Knoppix,
            OsNames.AspLinux,
            OsNames.Freebox,
            OsNames.Sabayon,
        };

        var osNames = OsParser.OperatingSystems
            .Select(os => os.Name)
            .ToHashSet();

        foreach (var name in OsParser.OsNameMapping.Keys.Except(ignoredNames))
        {
            osNames.ShouldContain(name);
        }
    }

    [Test]
    public void OsCodeMapping_ShouldContainAllOsCodes()
    {
        foreach (OsCode osCode in Enum.GetValues(typeof(OsCode)))
        {
            OsParser.OsCodeMapping.ShouldContainKey(osCode);
        }
    }

    [Test]
    public void OsFamilyMapping_ShouldContainAllOsCodes()
    {
        foreach (OsCode osCode in Enum.GetValues(typeof(OsCode)))
        {
            bool contains = false;

            foreach (var osFamily in OsParser.OsFamilyMapping)
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
    public void TryParse_WithFixtureData_ShouldReturnExpectedOsInfo()
    {
        var fixturePath = Path.Combine("Fixtures", "Resources", "operating_systems.yml");
        var fixtures = FixtureLoader.Load<OsFixture>(fixturePath);

        var parser = new OsParser(VersionTruncation.None);

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
            result.Platform.ShouldBe(fixture.Os.Platform);
            result.Family.ShouldBe(fixture.Os.Family);
        }
    }
}
