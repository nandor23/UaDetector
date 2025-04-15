using UaDetector.Results;

namespace UaDetector;

public interface IUaDetector
{
    bool TryParse(string userAgent, out UserAgentInfo? result);
    bool TryParse(string userAgent, IDictionary<string, string?> headers, out UserAgentInfo? result);
}
