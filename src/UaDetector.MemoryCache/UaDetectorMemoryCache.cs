using Microsoft.Extensions.Caching.Memory;

namespace UaDetector.MemoryCache;

public sealed class UaDetectorMemoryCache : IUaDetectorCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly UaDetectorMemoryCacheOptions _cacheOptions;
    private readonly MemoryCacheEntryOptions _entryOptions;

    public UaDetectorMemoryCache(UaDetectorMemoryCacheOptions cacheOptions)
    {
        _cacheOptions = cacheOptions;
        _memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(
            new MemoryCacheOptions
            {
                ExpirationScanFrequency = _cacheOptions.ExpirationScanFrequency,
                SizeLimit = _cacheOptions.MaxEntries,
                CompactionPercentage = _cacheOptions.EvictionPercentage,
            }
        );

        _entryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheOptions.Expiration,
            SlidingExpiration = _cacheOptions.SlidingExpiration,
        };

        if (_cacheOptions.MaxEntries is not null)
        {
            _entryOptions.Size = 1;
        }
    }

    public bool TryGet<T>(string key, out T? value)
    {
        if (_memoryCache.TryGetValue(key, out value))
        {
            return true;
        }

        value = default;
        return false;
    }

    public bool Set<T>(string key, T? value)
    {
        if (key.Length <= _cacheOptions.MaxKeyLength)
        {
            _memoryCache.Set(key, value, _entryOptions);
            return true;
        }

        return false;
    }
}
