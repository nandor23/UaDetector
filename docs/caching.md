## Caching

To enable caching, install the [UaDetector.MemoryCache](https://www.nuget.org/packages/UaDetector.MemoryCache) package and configure it using the `UseMemoryCache()` extension method.

```c#
using UaDetector;
using UaDetector.MemoryCache;

builder.Services.AddUaDetector(options =>
{
    options.UseMemoryCache();
});
```

### Configuration Options

| Option                    | Type        | Default                    | Description                                                                                                                                                                             |
|---------------------------|-------------|----------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `MaxKeyLength`            | `int`       | `256`                      | Maximum length allowed for a cache key. Entries with longer keys will not be cached.                                                                                                    |
| `Expiration`              | `TimeSpan?` | `null`                     | Entries will expire after this duration, regardless of how frequently they are accessed.                                                                                                |
| `SlidingExpiration`       | `TimeSpan?` | `null`                     | Entries will expire if they haven't been accessed within this time period. The expiration timer resets each time the entry is accessed.                                                 |
| `ExpirationScanFrequency` | `TimeSpan`  | <code>1&nbsp;minute</code> | Interval between automatic scans that evict expired cache entries.                                                                                                                      |
| `MaxEntries`              | `long?`     | `null`                     | Maximum number of entries allowed in the cache. When the limit is reached, least recently used entries will be evicted.                                                                 |
| `EvictionPercentage`      | `double`    | `0.05`                     | Percentage of cache entries to evict when `MaxEntries` limit is reached. Eviction runs asynchronously. When the cache is full, new entries will not be cached until eviction completes. |
