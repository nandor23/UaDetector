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
    public void TryGet_WhenEntryExpires_ShouldReturnNull()
    {
        const string key = "123";
        var cache = new UaDetectorMemoryCache(
            new UaDetectorMemoryCacheOptions
            {
                EntryExpiration = TimeSpan.FromMilliseconds(150),
                EntryExpirationScanFrequency = TimeSpan.FromMilliseconds(50),
            }
        );

        cache.Set(key, 23);

        Thread.Sleep(300);
        cache.TryGet(key, out int? result).ShouldBeFalse();
        result.ShouldBe(null);
    }

    [Test]
    public void TryGet_WhenSlidingExpirationHasElapsed_ShouldReturnNull()
    {
        const string key = "123";
        var cache = new UaDetectorMemoryCache(
            new UaDetectorMemoryCacheOptions
            {
                EntrySlidingExpiration = TimeSpan.FromMilliseconds(150),
                EntryExpirationScanFrequency = TimeSpan.FromMilliseconds(50),
            }
        );

        cache.Set(key, 23);

        Thread.Sleep(300);
        cache.TryGet(key, out int? result).ShouldBeFalse();
        result.ShouldBe(null);
    }
}
