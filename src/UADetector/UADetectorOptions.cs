using UADetector.Models.Enums;

namespace UADetector;

public class UADetectorOptions
{
    public VersionTruncationLevel VersionTruncation { get; set; } = VersionTruncationLevel.None;
}
