using BenchmarkDotNet.Attributes;

using DeviceDetectorNET;
using DeviceDetectorNET.Results;
using DeviceDetectorNET.Results.Client;

using UaDetector.Results;

using UAParser;

using ClientInfo = UAParser.ClientInfo;

namespace UaDetector.Benchmarks;

[MemoryDiagnoser]
public class ParserComparisonBenchmark
{
    private const string UserAgent = "Safari/9537.73.11 CFNetwork/673.0.3 Darwin/13.0.0 (x86_64) (MacBookAir6%2C2)";
    
    [Benchmark(Baseline = true)]
    public UserAgentInfo? UaDetector_TryParse()
    { 
        var uaDetector = new UaDetector();
        uaDetector.TryParse(UserAgent, out var result);
        return result;
    }
    
    [Benchmark]
    public ParseResult<BrowserMatchResult> DeviceDetector_Parse()
    { 
        var deviceDetector = new DeviceDetector(UserAgent);
        deviceDetector.Parse();
        return deviceDetector.GetBrowserClient();
    }
    
    [Benchmark]
    public ClientInfo UaParser_Parse()
    {
        var uaParser = Parser.GetDefault();
        return uaParser.Parse(UserAgent);
    }
}
