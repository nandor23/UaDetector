using Shouldly;

using UaDetector.Parsers.Clients;

namespace UaDetector.Tests.Tests.Parsers.Clients;

public class AppHintParserTests
{
    [Test]
    [MethodDataSource(nameof(AppHintTestData))]
    public void TryParseAppName_ShouldReturnExpectedAppName(string appHint, string? expectedAppName, bool result)
    {
        var headers = new Dictionary<string, string?>
        {
            {"x-requested-with", appHint}
        };

        var clientHints = ClientHints.Create(headers);

        AppHintParser.TryParseAppName(clientHints, out var appName).ShouldBe(result);
        appName.ShouldBe(expectedAppName);
    }

    public static IEnumerable<Func<(string, string?, bool)>> AppHintTestData()
    {
        yield return () => ("org.telegram.messenger", "Telegram", true);
        yield return () => ("com.instagram.android", "Instagram", true);
        yield return () => ("wrong.hint", null, false);
    }

}
