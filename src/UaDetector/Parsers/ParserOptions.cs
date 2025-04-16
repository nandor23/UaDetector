using UaDetector.Models.Enums;

namespace UaDetector.Parsers;

public sealed class ParserOptions
{
    public VersionTruncation VersionTruncation { get; set; } = VersionTruncation.Minor;
    public bool SkipBotParsing { get; set; }
}
