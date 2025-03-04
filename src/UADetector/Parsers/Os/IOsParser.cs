using UADetector.Results;

namespace UADetector.Parsers.Os;

public interface IOsParser
{
    bool TryParse(string userAgent, out OsInfo? result);
    bool TryParse(string userAgent, ClientHints? clientHints, out OsInfo? result);
}
