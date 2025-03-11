using UADetector.Results.Client;

namespace UADetector.Parsers;

public interface IClientParser
{
    bool TryParse(string userAgent, out IClientInfo? result);
    bool TryParse(string userAgent, ClientHints? clientHints, out IClientInfo? result);
}
