using UADetector.Models.Enums;

namespace UADetector.Parsers;

public sealed class ParserOptions
{
    public VersionTruncation VersionTruncation { get; set; } = VersionTruncation.Minor;
}
