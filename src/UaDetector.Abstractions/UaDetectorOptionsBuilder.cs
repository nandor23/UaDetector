using UaDetector.Abstractions.Enums;

namespace UaDetector.Abstractions;

public sealed class UaDetectorOptionsBuilder
{
    public VersionTruncation VersionTruncation { get; set; } = VersionTruncation.Minor;
    public bool DisableBotDetection { get; set; }
    private IUaDetectorCache? Cache { get; set; }

    public UaDetectorOptionsBuilder AddCache(IUaDetectorCache cache)
    {
        Cache = cache;
        return this;
    }

    public UaDetectorOptions Build()
    {
        return new UaDetectorOptions
        {
            VersionTruncation = VersionTruncation,
            DisableBotDetection = DisableBotDetection,
            Cache = Cache,
        };
    }
}
