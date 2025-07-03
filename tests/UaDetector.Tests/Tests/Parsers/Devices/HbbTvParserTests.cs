using Shouldly;
using UaDetector.Parsers.Devices;

namespace UaDetector.Tests.Tests.Parsers.Devices;

public class HbbTvParserTests
{
    [Test]
    public void HbbTvParserParser_Instantiation_ShouldNotThrowException()
    {
        Should.NotThrow(() => new HbbTvParser());
    }

    [Test]
    public void HbbTvParserParser_ShouldExtend_DeviceParserBase()
    {
        var parser = new HbbTvParser();
        parser.ShouldBeAssignableTo<DeviceParserBase>();
    }

    [Test]
    [MethodDataSource(nameof(UserAgentTestData))]
    public void HbbTvRegex_ShouldMatchUserAgent(string userAgent)
    {
        var match = HbbTvParser.HbbTvRegex.Match(userAgent);
        match.Success.ShouldBeTrue();
        match.Groups.Count.ShouldBe(2);
        match.Groups[1].Value.ShouldBe("1.1.1");
    }

    public static IEnumerable<Func<string>> UserAgentTestData()
    {
        yield return () =>
            "Opera/9.80 (Linux mips ; U; HbbTV/1.1.1 (; Philips; ; ; ; ) CE-HTML/1.0 NETTV/3.2.1; en) Presto/2.6.33 Version/10.70";
    }
}
