using UADetector.Models.Enums;

namespace UADetector;

public sealed class UADetectorOptions
{
    public VersionTruncation VersionTruncation { get; set; } = VersionTruncation.Minor;
    public bool SkipBotParsing { get; set; }
    public bool SkipBotDetails { get; set; }
}
