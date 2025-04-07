using Shouldly;

using UADetector.Models.Constants;
using UADetector.Parsers;

namespace UADetector.Tests.Parsers;

public class OsParserTests
{
    [Test]
    public void OsParser_Instantiation_ShouldNotThrowException()
    {
        Should.NotThrow(() => new OsParser());
    }

    [Test]
    public void OsParser_ShouldImplement_IOsParserInterface()
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
    public void OsNameMappings_ShouldMatchParsedOperatingSystems()
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

}
