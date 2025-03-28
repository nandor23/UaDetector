using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using UADetector.Parsers;

BenchmarkRunner.Run<TestThis>();

[MemoryDiagnoser]
public class TestThis
{
    [Benchmark]
    public void TestParser()
    {
        var userAgent = "VORTEX/1.2.3";
        var parser = new BotParser();
       // parser.TryParse(userAgent, out _);
       parser.IsBot(userAgent);
    }
}
