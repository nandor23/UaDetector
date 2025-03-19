using UADetector.Results;

namespace UADetector;

public interface IUADetector
{
    bool TryParse(string userAgent, out UserAgentInfo? result);
    bool TryParse(string userAgent, IDictionary<string, string?> headers, out UserAgentInfo? result);
}
