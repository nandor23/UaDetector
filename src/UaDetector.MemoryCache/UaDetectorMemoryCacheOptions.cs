namespace UaDetector.MemoryCache;

public sealed class UaDetectorMemoryCacheOptions
{
    /// <summary>
    /// Maximum length allowed for a cache key. Entries with longer keys will not be cached.
    /// </summary>
    public int MaxKeyLength { get; set; } = 256;

    /// <summary>
    /// Entries will expire after this duration, regardless of how frequently they are accessed.
    /// </summary>
    public TimeSpan? Expiration { get; set; }

    /// <summary>
    /// Entries will expire if they haven't been accessed within this time period.
    /// The expiration timer resets each time the entry is accessed.
    /// </summary>
    public TimeSpan? SlidingExpiration { get; set; }

    /// <summary>
    /// Interval between automatic scans that evict expired cache entries.
    /// </summary>
    public TimeSpan ExpirationScanFrequency { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Maximum number of entries allowed in the cache. When the limit is reached,
    /// least recently used entries will be evicted.
    /// </summary>
    public long? MaxEntries { get; set; }

    /// <summary>
    /// Percentage of cache entries to evict when MaxEntries limit is reached.
    /// This setting only applies when MaxEntries is configured.
    /// Default is 0.05 (5%). Higher values free more space during eviction.
    /// </summary>
    /// <remarks>
    /// Eviction runs asynchronously. When the cache is full, new entries
    /// are not added until eviction completes.
    /// </remarks>
    public double EvictionPercentage { get; set; } = 0.05;
}
