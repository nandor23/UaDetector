using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using UADetector.Parsers;
using UADetector.Results;

namespace UADetector;

public sealed class UADetector : IUADetector
{
    private readonly OsParser _osParser = new();


    public bool TryParse(string userAgent, [NotNullWhen(true)] out UserAgentInfo? result)
    {
        return TryParse(userAgent, ImmutableDictionary<string, string?>.Empty, out result);
    }

    public bool TryParse(string userAgent, IDictionary<string, string?> headers, [NotNullWhen(true)] out UserAgentInfo? result)
    {
        var clientHints = ClientHints.Create(headers);

        if (ParserExtensions.TryRestoreUserAgent(userAgent, clientHints, out var restoredUserAgent))
        {
            userAgent = restoredUserAgent;
        }

        throw new NotImplementedException();
    }
}
