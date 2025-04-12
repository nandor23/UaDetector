using Shouldly;

using UADetector.Parsers.Devices;

namespace UADetector.Tests.Tests.Parsers.Devices;

public class ShellTvParserTests
{
    [Test]
    public void ShellTvParserParser_Instantiation_ShouldNotThrowException()
    {
        Should.NotThrow(() => new ShellTvParser());
    }

    [Test]
    public void ShellTvParserParser_ShouldExtend_DeviceParserBase()
    {
        var parser = new ShellTvParser();
        parser.ShouldBeAssignableTo<DeviceParserBase>();
    }

    [Test]
    [MethodDataSource(nameof(UserAgentTestData))]
    public void ShellTvRegex_ShouldMatchUserAgent(string userAgent, bool result)
    {
        var isMatch = ShellTvParser.ShellTvRegex.IsMatch(userAgent);
        isMatch.ShouldBe(result);
    }

    public static IEnumerable<Func<(string, bool)>> UserAgentTestData()
    {
        yield return () => ("Leff Shell LC390TA2A", true);
        yield return () => ("Leff Shell", false);
    }
}
