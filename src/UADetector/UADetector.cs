using System.Diagnostics.CodeAnalysis;

using UADetector.Parsers;
using UADetector.Results;

namespace UADetector;

public sealed class UADetector : IUADetector
{
    private readonly IOsParser _osParser;


    public UADetector(IOsParser osParser)
    {
        _osParser = osParser;
    }

    public bool TryParse(string userAgent, [NotNullWhen(true)] out UserAgentInfo? result)
    {
        return TryParse(userAgent, null, out result);
    }

    public bool TryParse(string userAgent, ClientHints? clientHints, [NotNullWhen(true)] out UserAgentInfo? result)
    {
        if (clientHints is not null &&
            ParserExtensions.TryRestoreUserAgent(userAgent, clientHints, out var restoredUserAgent))
        {
            userAgent = restoredUserAgent;
        }

        throw new NotImplementedException();
    }
}
