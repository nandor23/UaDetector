namespace UaDetector;

public sealed class BotParserOptionsBuilder
{
    internal readonly BotParserOptions ParserOptions = new();

    public BotParserOptionsBuilder AddCache(IUaDetectorCache cache)
    {
        ParserOptions.Cache = cache;
        return this;
    }
}
