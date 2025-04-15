using UaDetector.Results;

namespace UaDetector.Parsers;

public interface IBrowserParser
{
    bool TryParse(string userAgent, out BrowserInfo? result);
    bool TryParse(string userAgent, IDictionary<string, string?> headers, out BrowserInfo? result);
}
