using UaDetector.Abstractions.Enums;

namespace UaDetector.Abstractions;

public sealed class UaDetectorOptions
{
    public VersionTruncation VersionTruncation { get; set; } = VersionTruncation.Minor;
    public bool DisableBotDetection { get; set; }
    public IUaDetectorCache? Cache { get; set; }
}
