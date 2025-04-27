using UaDetector.Models.Enums;

namespace UaDetector;

public sealed class UaDetectorOptions
{
    public VersionTruncation VersionTruncation { get; set; } = VersionTruncation.Minor;
    public bool DisableBotDetection { get; set; }
}
