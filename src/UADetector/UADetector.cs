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

    public UserAgentInfo Parse(string userAgent, ClientHints? clientHints = null)
    {
        if (clientHints is not null &&
            ParserExtensions.TryRestoreUserAgent(userAgent, clientHints, out var result))
        {
            userAgent = result;
        }

        //   _osParser.TryParse(userAgent, null);

        return new UserAgentInfo();
    }
}
