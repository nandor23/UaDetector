using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

using DeviceDetectorNET;
using DeviceDetectorNET.Results;
using DeviceDetectorNET.Results.Client;
using UaDetector.Abstractions.Models;
using UAParser;
using ClientInfo = UAParser.ClientInfo;

namespace UaDetector.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class LibraryComparisonBenchmark
{
    private string[] _userAgents = null!;
    private int _index;

    [GlobalSetup]
    public void Setup()
    {
        _userAgents = TestUserAgents.All;
        _index = 0;
    }

    [Benchmark(Baseline = true)]
    public UserAgentInfo? UaDetector()
    {
        var ua = _userAgents[_index++ % _userAgents.Length];
        var uaDetector = new UaDetector();
        uaDetector.TryParse(ua, out var result);
        return result;
    }

    [Benchmark(Description = "DeviceDetector.NET")]
    public ParseResult<BrowserMatchResult> DeviceDetector()
    {
        var ua = _userAgents[_index++ % _userAgents.Length];
        var deviceDetector = new DeviceDetector(ua);
        deviceDetector.Parse();
        return deviceDetector.GetBrowserClient();
    }

    [Benchmark]
    public ClientInfo UAParser()
    {
        var ua = _userAgents[_index++ % _userAgents.Length];
        var uaParser = Parser.GetDefault();
        return uaParser.Parse(ua);
    }
}
