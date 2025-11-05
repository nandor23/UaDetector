using Shouldly;
using UaDetector.Parsers.Clients;

namespace UaDetector.Tests.Tests.Parsers.Clients;

public class AppHintParserTests
{
    [Test]
    [Arguments("org.telegram.messenger", "Telegram", true)]
    [Arguments("com.instagram.android", "Instagram", true)]
    [Arguments("wrong.hint", null, false)]
    public void TryParseAppName_ShouldReturnExpectedAppName(
        string appHint,
        string? expectedAppName,
        bool result
    )
    {
        var headers = new Dictionary<string, string?> { { "x-requested-with", appHint } };

        var clientHints = ClientHints.Create(headers);

        AppHintParser.TryParseAppName(clientHints, out var appName).ShouldBe(result);
        appName.ShouldBe(expectedAppName);
    }
}
