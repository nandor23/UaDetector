using UaDetector.Results;

namespace UaDetector.Parsers;

public interface IClientParser
{
    bool TryParse(string userAgent, out ClientInfo? result);
    bool TryParse(string userAgent, IDictionary<string, string?> headers, out ClientInfo? result);
}
