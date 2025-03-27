using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using UADetector.Parsers;

BenchmarkRunner.Run<ParserTest>();


[MemoryDiagnoser]
public class ParserTest
{
    private const string UserAgent =
        "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36";

    private readonly BotParser _parser = new();

    [Benchmark]
    public void Parser()
    {
        _parser.TryParse(UserAgent, out _);
    }
}
