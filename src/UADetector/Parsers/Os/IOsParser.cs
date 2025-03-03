using UADetector.Results;

namespace UADetector.Parsers.Os;

public interface IOsParser
{
    OsInfo Parse(string userAgent, ClientHints? clientHints);
}
