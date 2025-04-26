using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using UaDetector.Parsers.Clients;
using UaDetector.Results;

namespace UaDetector.Parsers;

public sealed class ClientParser : IClientParser
{
    private readonly ParserOptions _parserOptions;
    private readonly BotParser _botParser;
    internal readonly IEnumerable<ClientParserBase> ClientParsers;

    public ClientParser(ParserOptions? parserOptions = null)
    {
        _parserOptions = parserOptions ?? new ParserOptions();

        ClientParsers =
        [
            new FeedReaderParser(_parserOptions.VersionTruncation),
            new MobileAppParser(_parserOptions.VersionTruncation),
            new MediaPlayerParser(_parserOptions.VersionTruncation),
            new PimParser(_parserOptions.VersionTruncation),
            new LibraryParser(_parserOptions.VersionTruncation),
        ];

        _botParser = new BotParser();
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
        if (!_parserOptions.DisableBotDetection && _botParser.IsBot(userAgent))
        {
            result = null;
            return false;
        }

        var clientHints = ClientHints.Create(headers);

        if (ParserExtensions.TryRestoreUserAgent(userAgent, clientHints, out var restoredUserAgent))
        {
            userAgent = restoredUserAgent;
        }

        return TryParse(userAgent, clientHints, out result);
    }

    internal bool TryParse(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out ClientInfo? result
    )
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

    internal bool IsClient(string userAgent, ClientHints clientHints)
    {
        foreach (var parser in ClientParsers)
        {
            if (parser.IsClient(userAgent, clientHints))
            {
                return true;
            }
        }

        return false;
    }
}
