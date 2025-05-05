namespace UaDetector.MemoryCache;

public static class UaDetectorOptionsBuilderExtensions
{
    public static void UseMemoryCache(
        this UaDetectorOptionsBuilder optionsBuilder,
        Action<UaDetectorMemoryCacheOptions>? cacheOptionsAction = null
    )
    {
        var cacheOptions = new UaDetectorMemoryCacheOptions();
        cacheOptionsAction?.Invoke(cacheOptions);

        var uaDetectorCache = new UaDetectorMemoryCache(cacheOptions);
        optionsBuilder.AddCache(uaDetectorCache);
    }
}
