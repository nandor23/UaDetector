namespace UaDetector;

public abstract class UaDetectorCacheOptions
{
    /// <summary>
    /// Prefix applied to all cache keys.
    /// </summary>
    public string KeyPrefix { get; set; } = string.Empty;

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
}
