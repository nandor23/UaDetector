using UADetector.Parsers;
using UADetector.Parsers.Os;
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
        ParserExtensions.TryRestoreUserAgentFromClientHints(userAgent, clientHints, out var result);

        if (result is not null)
        {
            userAgent = result;
        }

        //   _osParser.TryParse(userAgent, null);

        return new UserAgentInfo();
    }
}
