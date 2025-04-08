using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using UADetector.Models.Enums;
using UADetector.Parsers.Clients;
using UADetector.Results;

namespace UADetector.Parsers;

public sealed class ClientParser : IClientParser
{
    private readonly IEnumerable<ClientParserBase> _clientParsers;


    public ClientParser(VersionTruncation versionTruncation = VersionTruncation.Minor)
    {
        _clientParsers = [
            new MobileAppParser(versionTruncation),
            new MediaPlayerParser(versionTruncation),
            new LibraryParser(versionTruncation),
            new FeedReaderParser(versionTruncation),
            new PimParser(versionTruncation)
        ];
    }

    public bool TryParse(string userAgent, [NotNullWhen(true)] out ClientInfo? result)
    {
        return TryParse(userAgent, ImmutableDictionary<string, string?>.Empty, out result);
    }

    public bool TryParse(
        string userAgent,
        IDictionary<string, string?> headers,
        [NotNullWhen(true)] out ClientInfo? result
    )
    {
        var clientHints = ClientHints.Create(headers);
        return TryParse(userAgent, clientHints, out result);
    }

    internal bool TryParse(string userAgent, ClientHints clientHints, [NotNullWhen(true)] out ClientInfo? result)
    {
        foreach (var parser in _clientParsers)
        {
            if (parser.TryParse(userAgent, clientHints, out result))
            {
                return true;
            }
        }

        result = null;
        return false;
    }
}
