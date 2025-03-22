using System.Diagnostics.CodeAnalysis;

using UADetector.Models.Enums;
using UADetector.Parsers.Clients;
using UADetector.Results;

namespace UADetector.Parsers;

public sealed class ClientParser : IClientParser
{
    private readonly IEnumerable<BaseClientParser> _parsers;


    public ClientParser(VersionTruncation versionTruncation = VersionTruncation.Minor)
    {
        _parsers = [
            new MobileAppParser(versionTruncation),
            new MediaPlayerParser(versionTruncation),
            new LibraryParser(versionTruncation),
            new FeedReaderParser(versionTruncation),
            new PimParser(versionTruncation)
        ];
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
