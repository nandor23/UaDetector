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
    public TimeSpan? EntryExpiration { get; set; }

    /// <summary>
    /// Entries will expire if they haven't been accessed within this time period.
    /// The expiration timer resets each time the entry is accessed.
    /// </summary>
    public TimeSpan? EntrySlidingExpiration { get; set; }

    /// <summary>
    /// Interval between automatic scans that remove expired cache entries.
    /// </summary>
    public TimeSpan EntryExpirationScanFrequency = TimeSpan.FromMinutes(1);
}
