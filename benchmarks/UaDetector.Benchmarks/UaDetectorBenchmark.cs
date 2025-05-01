using BenchmarkDotNet.Attributes;

using UaDetector.Parsers;
using UaDetector.Results;

namespace UaDetector.Benchmarks;

[MemoryDiagnoser]
public class UaDetectorBenchmark
{
    [Benchmark]
    public UserAgentInfo? UaDetector_TryParse()
    {
        var uaDetector = new UaDetector();
        var userAgent =
            "Safari/9537.73.11 CFNetwork/673.0.3 Darwin/13.0.0 (x86_64) (MacBookAir6%2C2)";

        uaDetector.TryParse(userAgent, out var result);

        return result;
    }

    [Benchmark]
    public BrowserInfo? BrowserParser_TryParse()
    {
        var parser = new BrowserParser();
        var userAgent =
            "Safari/9537.73.11 CFNetwork/673.0.3 Darwin/13.0.0 (x86_64) (MacBookAir6%2C2)";

        parser.TryParse(userAgent, out var result);
        return result;
    }

    [Benchmark]
    public ClientInfo? ClientParser_TryParse()
    {
        var parser = new ClientParser();
        var userAgent = "Siri/1 CFNetwork/1128.0.1 Darwin/19.6.0";

        parser.TryParse(userAgent, out var result);

        return result;
    }

    [Benchmark]
    public BotInfo? BotParser_TryParse()
    {
        var parser = new BotParser();
        var userAgent =
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1; 360Spider";

        parser.TryParse(userAgent, out var result);

        return result;
    }

    [Benchmark]
    public bool BotParser_IsBot()
    {
        var parser = new BotParser();
        var userAgent =
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1; 360Spider";

        return parser.IsBot(userAgent);
    }
}
