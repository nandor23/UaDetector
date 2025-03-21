using System.Diagnostics.CodeAnalysis;

using UADetector.Results;

namespace UADetector.Parsers;

public class ClientParser : IClientParser
{
    public ClientParser(ParserOptions? parserOptions = null)
    {
        var options = parserOptions ?? new ParserOptions();
    }

    public bool TryParse(string userAgent, [NotNullWhen(true)] out ClientInfo? result)
    {
        throw new NotImplementedException();
    }

    public bool TryParse(string userAgent, IDictionary<string, string?> headers, [NotNullWhen(true)] out ClientInfo? result)
    {
        throw new NotImplementedException();
    }
}
