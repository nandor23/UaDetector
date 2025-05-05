namespace UaDetector;

public sealed class BotParserOptionsBuilder
{
    private IUaDetectorCache? Cache { get; set; }

    public BotParserOptionsBuilder AddCache(IUaDetectorCache cache)
    {
        Cache = cache;
        return this;
    }

    internal BotParserOptions Build()
    {
        return new BotParserOptions { Cache = Cache };
    }
}
