using Shouldly;

namespace UaDetector.MemoryCache.Tests;

public class UaDetectorMemoryCacheTests
{
    [Test]
    public void MemoryCache_ShouldCacheValue()
    {
        const string key = "123";
        const int value = 23;
        var cache = new UaDetectorMemoryCache(new UaDetectorMemoryCacheOptions());

        cache.Set(key, value);

        cache.TryGet(key, out int? result).ShouldBeTrue();
        result.ShouldBe(value);
    }

    [Test]
    public void MemoryCache_WithKeyPrefix_ShouldCacheValue()
    {
        const string key = "123";
        const int value = 23;
        var cache = new UaDetectorMemoryCache(
            new UaDetectorMemoryCacheOptions { KeyPrefix = "prefix" }
        );

        cache.Set(key, value);

        cache.TryGet(key, out int? result).ShouldBeTrue();
        result.ShouldBe(value);
    }
}
