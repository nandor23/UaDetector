using Shouldly;
using UaDetector.Parsers;

namespace UaDetector.Tests.Tests.Parsers;

public class ParserExtensionsTests
{
    [Test]
    [MethodDataSource(nameof(UserAgentTestData))]
    public void HasUserAgentClientHintsFragment_ReturnsExpectedResult(
        string userAgent,
        bool expectedResult
    )
    {
        var result = ParserExtensions.HasUserAgentClientHintsFragment(userAgent);

        result.ShouldBe(expectedResult);
    }

    public static IEnumerable<(string useragent, bool result)> UserAgentTestData()
    {
        yield return (
            "Mozilla/5.0 (Linux; Android 9; K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.7204.180 Mobile Safari/537.36 Telegram-Android/12.2.10 (Zte ZTE Blade A3 2020RU; Android 9; SDK 28; LOW)",
            false
        );
        yield return (
            "Mozilla/5.0 (Linux; Android 10; K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.7444.171 Mobile Safari/537.36 Telegram-Android/12.2.7 (Itel itel W5006X; Android 10; SDK 29; LOW)",
            false
        );
        yield return (
            "Mozilla/5.0 (Linux; Android 16; K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.7444.171 Mobile Safari/537.36",
            true
        );
        yield return (
            "Mozilla/5.0 (Linux; Android 14; K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Mobile Safari/537.36",
            true
        );
        yield return (
            "Mozilla/5.0 (Linux; Android 11) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/126.0.0.0 Mobile DuckDuckGo/5 Safari/537.36",
            true
        );
        yield return (
            "Mozilla/5.0 (Linux; Android 15; K) Telegram-Android/12.2.10 (Tecno TECNO CL6; Android 15; SDK 35; AVERAGE)",
            false
        );
        yield return (
            "Mozilla/5.0 (Linux; Android 10; K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Mobile Safari/537.36",
            true
        );
        yield return (
            "Mozilla/5.0 (Linux; Android 10; K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Mobile Safari/537.36 AlohaBrowser/5.10.4",
            true
        );
        yield return (
            "Mozilla/5.0 (Linux; Android 10; K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.227.6834 Safari/537.36  SberBrowser/3.4.0.1123",
            true
        );
        yield return (
            "Mozilla/5.0 (Linux; Android 14; K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.7232.2 Mobile Safari/537.36 YaApp_Android/22.116.1 YaSearchBrowser/9.20",
            true
        );
        yield return (
            "Mozilla/5.0 (Linux; Android 10; K) AppleWebKit/537.36 (KHTML, like G -ecko) Chrome/142.0.0.0 Safari/537.36 EdgA/142.0.0.0",
            true
        );
        yield return (
            "Mozilla/5.0 (Linux; Android 10; K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.6312.118 Mobile Safari/537.36 XiaoMi/MiuiBrowser/14.33.0-gn",
            true
        );
    }
}
