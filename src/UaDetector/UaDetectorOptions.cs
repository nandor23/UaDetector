using UaDetector.Models.Enums;

namespace UaDetector;

public sealed class UaDetectorOptions
{
    public VersionTruncation VersionTruncation { get; set; } = VersionTruncation.Minor;
    public bool SkipBotParsing { get; set; }
    public bool SkipBotDetails { get; set; }
}
