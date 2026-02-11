using BenchmarkDotNet.Attributes;
using UaDetector.Abstractions.Models;
using UaDetector.Parsers;

namespace UaDetector.Benchmarks;

[MemoryDiagnoser]
public class UaDetectorBenchmark
{
    private string[] _userAgents = null!;
    private int _index;
    private UaDetector _uaDetector = null!;

    [GlobalSetup]
    public void Setup()
    {
        _userAgents = TestUserAgents.All;
        _index = 0;
        _uaDetector = new UaDetector();

        // Warm up - trigger regex compilation
        _uaDetector.TryParse("uadetector-warmup", out _);
    }

    [Benchmark]
    public UserAgentInfo? UaDetector_TryParse()
    {
        var ua = _userAgents[_index++ % _userAgents.Length];
        _uaDetector.TryParse(ua, out var result);
        return result;
    }

    [Benchmark]
    public OsInfo? OsParser_TryParse()
    {
        var parser = new OsParser();
        var ua = _userAgents[_index++ % _userAgents.Length];
        parser.TryParse(ua, out var result);
        return result;
    }

    [Benchmark]
    public BrowserInfo? BrowserParser_TryParse()
    {
        var parser = new BrowserParser();
        var ua = _userAgents[_index++ % _userAgents.Length];
        parser.TryParse(ua, out var result);
        return result;
    }

    [Benchmark]
    public ClientInfo? ClientParser_TryParse()
    {
        var parser = new ClientParser();
        var ua = _userAgents[_index++ % _userAgents.Length];
        parser.TryParse(ua, out var result);
        return result;
    }

    [Benchmark]
    public BotInfo? BotParser_TryParse()
    {
        var parser = new BotParser();
        var ua = _userAgents[_index++ % _userAgents.Length];
        parser.TryParse(ua, out var result);
        return result;
    }

    [Benchmark]
    public bool BotParser_IsBot()
    {
        var parser = new BotParser();
        var ua = _userAgents[_index++ % _userAgents.Length];
        return parser.IsBot(ua);
    }
}
