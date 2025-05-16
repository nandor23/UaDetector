using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using UaDetector.Parsers.Clients;
using UaDetector.Results;

namespace UaDetector.Parsers;

public sealed class ClientParser : IClientParser
{
    private const string CacheKeyPrefix = "client";
    private readonly IUaDetectorCache? _cache;
    private readonly UaDetectorOptions _uaDetectorOptions;
    private readonly BotParser _botParser;
    private readonly ParserHelper _parserHelper;
    internal readonly IReadOnlyList<ClientParserBase> ClientParsers;

    public ClientParser(UaDetectorOptions? uaDetectorOptions = null)
    {
        _uaDetectorOptions = uaDetectorOptions ?? new UaDetectorOptions();
        _cache = uaDetectorOptions?.Cache;
        _botParser = new BotParser(new BotParserOptions { Cache = _cache });
        _parserHelper = new ParserHelper();

        ClientParsers =
        [
            new FeedReaderParser(_uaDetectorOptions.VersionTruncation),
            new MobileAppParser(_uaDetectorOptions.VersionTruncation),
            new MediaPlayerParser(_uaDetectorOptions.VersionTruncation),
            new PimParser(_uaDetectorOptions.VersionTruncation),
            new LibraryParser(_uaDetectorOptions.VersionTruncation),
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
        if (!_uaDetectorOptions.DisableBotDetection && _botParser.IsBot(userAgent))
        {
            result = null;
            return false;
        }

        var clientHints = ClientHints.Create(headers);

        if (_parserHelper.TryRestoreUserAgent(userAgent, clientHints, out var restoredUserAgent))
        {
            userAgent = restoredUserAgent;
        }

        var cacheKey = $"{CacheKeyPrefix}:{userAgent}";

        if (_cache is not null && _cache.TryGet(cacheKey, out result))
        {
            return result is not null;
        }

        TryParse(userAgent, clientHints, out result);
        _cache?.Set(cacheKey, result);
        return result is not null;
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
