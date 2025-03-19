using UADetector.Results.Client;

namespace UADetector.Parsers;

public interface IClientParser
{
    bool TryParse(string userAgent, out IClientInfo? result);
    bool TryParse(string userAgent, IDictionary<string, string?> headers, out IClientInfo? result);
}
