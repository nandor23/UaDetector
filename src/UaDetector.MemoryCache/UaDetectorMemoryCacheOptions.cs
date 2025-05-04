namespace UaDetector.MemoryCache;

public sealed class UaDetectorMemoryCacheOptions : UaDetectorCacheOptions
{
    public TimeSpan EntryExpirationScanFrequency = TimeSpan.FromMinutes(1);
}
