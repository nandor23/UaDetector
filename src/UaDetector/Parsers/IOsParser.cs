using System.Diagnostics.CodeAnalysis;
using UaDetector.Models;

namespace UaDetector.Parsers;

public interface IOsParser
{
    bool TryParse(string userAgent, [NotNullWhen(true)] out OsInfo? result);
    bool TryParse(
        string userAgent,
        IDictionary<string, string?> headers,
        [NotNullWhen(true)] out OsInfo? result
    );
}
