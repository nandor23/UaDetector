using UADetector.Results;

namespace UADetector.Parsers;

public interface IBotParser
{
    bool TryParse(string userAgent, out BotInfo? result);
}
