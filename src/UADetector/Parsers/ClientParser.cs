using System.Diagnostics.CodeAnalysis;

using UADetector.Parsers.Client;
using UADetector.Results;

namespace UADetector.Parsers;

public class ClientParser : IClientParser
{
    private readonly BrowserParser _browserParser = new();

    public bool TryParse(string userAgent, [NotNullWhen(true)] out ClientInfo? result)
    {
        _browserParser.TryParse(userAgent, null, out var res);

        throw new NotImplementedException();
    }

    public bool TryParse(string userAgent, ClientHints? clientHints, [NotNullWhen(true)] out ClientInfo? result)
    {
        _browserParser.TryParse(userAgent, clientHints, out var res);

        throw new NotImplementedException();
    }
}
