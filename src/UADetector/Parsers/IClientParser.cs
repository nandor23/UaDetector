using UADetector.Results;

namespace UADetector.Parsers;

public interface IClientParser
{
    bool TryParse(string userAgent, out ClientInfo? result);
    bool TryParse(string userAgent, ClientHints? clientHints, out ClientInfo? result);
}
