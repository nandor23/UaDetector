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
    public void Set_WhenKeyLengthExceedsLimit_ShouldNotCacheValue()
    {
        const string key = "123";
        var cache = new UaDetectorMemoryCache(
            new UaDetectorMemoryCacheOptions { MaxKeyLength = 1 }
        );

        cache.Set(key, 23).ShouldBeFalse();
    }

    [Test]
    public void TryGet_WhenEntryExpires_ShouldReturnFalse()
    {
        const string key = "123";
        var cache = new UaDetectorMemoryCache(
            new UaDetectorMemoryCacheOptions
            {
                Expiration = TimeSpan.FromMilliseconds(150),
                ExpirationScanFrequency = TimeSpan.FromMilliseconds(50),
            }
        );

        cache.Set(key, 23);

        Thread.Sleep(300);
        cache.TryGet(key, out int? result).ShouldBeFalse();
        result.ShouldBe(null);
    }

    [Test]
    public void TryGet_WhenSlidingExpirationHasElapsed_ShouldReturnFalse()
    {
        const string key = "123";
        var cache = new UaDetectorMemoryCache(
            new UaDetectorMemoryCacheOptions
            {
                SlidingExpiration = TimeSpan.FromMilliseconds(150),
                ExpirationScanFrequency = TimeSpan.FromMilliseconds(50),
            }
        );

        cache.Set(key, 23);

        Thread.Sleep(300);
        cache.TryGet(key, out int? result).ShouldBeFalse();
        result.ShouldBe(null);
    }

    [Test]
    public void TryGet_WhenCachedValueIsNull_ShouldReturnTrue()
    {
        const string key = "123";
        int? value = null;
        var cache = new UaDetectorMemoryCache(new UaDetectorMemoryCacheOptions());

        cache.Set(key, value);

        cache.TryGet(key, out int? result).ShouldBeTrue();
        result.ShouldBe(value);
    }
}
