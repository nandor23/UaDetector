using UaDetector.Results;

namespace UaDetector.Parsers;

public interface IBotParser
{
    bool TryParse(string userAgent, out BotInfo? result);
}
