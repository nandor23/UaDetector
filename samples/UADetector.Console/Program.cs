using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<RegexBenchmark>();



public class RegexBenchmark
{
    [Benchmark]
    public bool IsMatch_Normal()
    {
        return true;
    }
}

