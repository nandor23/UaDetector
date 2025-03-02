using UADetector.Models.Enums;

namespace UADetector;

public class UADetectorOptions
{
    public VersionTruncation VersionTruncation { get; set; } = VersionTruncation.Minor;
}
