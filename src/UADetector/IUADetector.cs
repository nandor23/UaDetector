using UADetector.Results;

namespace UADetector;

public interface IUADetector
{ 
    UserAgentInfo Parse(string userAgent, ClientHints clientHints);
}
