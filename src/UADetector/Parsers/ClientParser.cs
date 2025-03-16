using System.Diagnostics.CodeAnalysis;

using UADetector.Parsers.Client;
using UADetector.Results.Client;

namespace UADetector.Parsers;

public class ClientParser : IClientParser
{
    private readonly BrowserParser _browserParser;

    public ClientParser(ParserOptions? parserOptions = null)
    {
        var options = parserOptions ?? new ParserOptions();
        _browserParser = new BrowserParser(options);
    }

    public bool TryParse(string userAgent, [NotNullWhen(true)] out IClientInfo? result)
    {
        _browserParser.TryParse(userAgent, null, out var res);

        throw new NotImplementedException();
    }

    public bool TryParse(string userAgent, ClientHints? clientHints, [NotNullWhen(true)] out IClientInfo? result)
    {
        _browserParser.TryParse(userAgent, clientHints, out var res);

        throw new NotImplementedException();
    }
}
