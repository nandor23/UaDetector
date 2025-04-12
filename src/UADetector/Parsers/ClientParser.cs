using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using UADetector.Models.Enums;
using UADetector.Parsers.Clients;
using UADetector.Results;

namespace UADetector.Parsers;

public sealed class ClientParser : IClientParser
{
    internal readonly IEnumerable<ClientParserBase> ClientParsers;


    public ClientParser(VersionTruncation versionTruncation = VersionTruncation.Minor)
    {
        ClientParsers = [
            new FeedReaderParser(versionTruncation),
            new MobileAppParser(versionTruncation),
            new MediaPlayerParser(versionTruncation),
            new PimParser(versionTruncation),
            new LibraryParser(versionTruncation),
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
        foreach (var parser in ClientParsers)
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
