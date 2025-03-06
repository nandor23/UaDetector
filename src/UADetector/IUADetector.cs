using UADetector.Results;

namespace UADetector;

public interface IUADetector
{
    bool TryParse(string userAgent, out UserAgentInfo? result);
    bool TryParse(string userAgent, ClientHints? clientHints, out UserAgentInfo? result);
}
