using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using DeviceDetectorNET;
using UAParser;

namespace UaDetector.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class LibraryComparisonBenchmark
{
    private string[] _userAgents = null!;
    private UaDetector _uaDetector = null!;
    private Parser _uaParser = null!;

    [GlobalSetup]
    public void Setup()
    {
        _userAgents = TestUserAgents.All;
        _uaDetector = new UaDetector();
        _uaParser = Parser.GetDefault();

        if (_userAgents.Length != TestUserAgents.Count)
        {
            throw new InvalidOperationException(
                $"TestUserAgents.Count ({TestUserAgents.Count}) must equal TestUserAgents.All.Length ({_userAgents.Length})."
            );
        }

        // Warm up - trigger regex compilation
        _uaDetector.TryParse("uadetector-warmup", out _);
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = TestUserAgents.Count)]
    public void UaDetector()
    {
        foreach (var ua in _userAgents)
        {
            _uaDetector.TryParse(ua, out _);
        }
    }

    [Benchmark(Description = "DeviceDetector.NET", OperationsPerInvoke = TestUserAgents.Count)]
    public void DeviceDetector()
    {
        foreach (var ua in _userAgents)
        {
            var deviceDetector = new DeviceDetector(ua);
            deviceDetector.Parse();
            deviceDetector.GetBrowserClient();
        }
    }

    [Benchmark(OperationsPerInvoke = TestUserAgents.Count)]
    public void UAParser()
    {
        foreach (var ua in _userAgents)
        {
            _uaParser.Parse(ua);
        }
    }
}
