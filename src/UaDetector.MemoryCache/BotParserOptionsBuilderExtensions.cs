namespace UaDetector.MemoryCache;

public static class BotParserOptionsBuilderExtensions
{
    public static void UseMemoryCache(
        this BotParserOptionsBuilder optionsBuilder,
        Action<UaDetectorMemoryCacheOptions>? cacheOptionsAction = null
    )
    {
        var cacheOptions = new UaDetectorMemoryCacheOptions();
        cacheOptionsAction?.Invoke(cacheOptions);

        var uaDetectorCache = new UaDetectorMemoryCache(cacheOptions);
        optionsBuilder.AddCache(uaDetectorCache);
    }
}
