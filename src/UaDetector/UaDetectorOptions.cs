using UaDetector.Abstractions.Enums;

namespace UaDetector;

public sealed class UaDetectorOptions
{
    public VersionTruncation VersionTruncation { get; set; } = VersionTruncation.Minor;
    public bool DisableBotDetection { get; set; }
    internal IUaDetectorCache? Cache { get; set; }
}
