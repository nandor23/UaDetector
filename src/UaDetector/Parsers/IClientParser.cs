using System.Diagnostics.CodeAnalysis;
using UaDetector.Models;

namespace UaDetector.Parsers;

public interface IClientParser
{
    bool TryParse(string userAgent, [NotNullWhen(true)] out ClientInfo? result);
    bool TryParse(
        string userAgent,
        IDictionary<string, string?> headers,
        [NotNullWhen(true)] out ClientInfo? result
    );
}
