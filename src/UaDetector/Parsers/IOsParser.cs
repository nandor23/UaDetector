using UaDetector.Results;

namespace UaDetector.Parsers;

public interface IOsParser
{
    bool TryParse(string userAgent, out OsInfo? result);
    bool TryParse(string userAgent, IDictionary<string, string?> headers, out OsInfo? result);
}
