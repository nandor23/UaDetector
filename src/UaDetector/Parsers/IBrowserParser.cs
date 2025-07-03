using System.Diagnostics.CodeAnalysis;
using UaDetector.Abstractions.Models;

namespace UaDetector.Parsers;

public interface IBrowserParser
{
    bool TryParse(string userAgent, [NotNullWhen(true)] out BrowserInfo? result);
    bool TryParse(
        string userAgent,
        IDictionary<string, string?> headers,
        [NotNullWhen(true)] out BrowserInfo? result
    );
}
