using System.Diagnostics.CodeAnalysis;
using UaDetector.Abstractions.Models;

namespace UaDetector;

public interface IUaDetector
{
    bool TryParse(string userAgent, [NotNullWhen(true)] out UserAgentInfo? result);

    bool TryParse(
        string userAgent,
        IDictionary<string, string?> headers,
        [NotNullWhen(true)] out UserAgentInfo? result
    );
}
