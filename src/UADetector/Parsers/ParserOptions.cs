using UADetector.Models.Enums;

namespace UADetector.Parsers;

public class ParserOptions
{
    public VersionTruncation VersionTruncation { get; set; } = VersionTruncation.Minor;
}
