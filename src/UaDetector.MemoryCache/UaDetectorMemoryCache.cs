using System.Diagnostics.CodeAnalysis;
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
                ExpirationScanFrequency = _cacheOptions.EntryExpirationScanFrequency,
            }
        );

        _entryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheOptions.EntryExpiration,
            SlidingExpiration = _cacheOptions.EntrySlidingExpiration,
        };
    }

    public bool TryGet<T>(string key, [NotNullWhen(true)] out T? value)
    {
        if (!_memoryCache.TryGetValue(key, out value))
        {
            value = default;
        }

        return value is not null;
    }

    public bool Set<T>(string key, T value)
    {
        if (key.Length <= _cacheOptions.MaxKeyLength)
        {
            _memoryCache.Set(key, value, _entryOptions);
            return true;
        }

        return false;
    }
}
