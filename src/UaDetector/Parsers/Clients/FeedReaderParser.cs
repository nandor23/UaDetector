using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Abstractions.Enums;
using UaDetector.Abstractions.Models;
using UaDetector.Attributes;
using UaDetector.Models;

namespace UaDetector.Parsers.Clients;

internal sealed partial class FeedReaderParser : ClientParserBase
{
    [RegexSource("Resources/Clients/feed_readers.json")]
    internal static partial IReadOnlyList<Client> FeedReaders { get; }

    [CombinedRegex]
    private static partial Regex CombinedRegex { get; }

    public FeedReaderParser(VersionTruncation versionTruncation)
        : base(versionTruncation) { }

    public override bool IsClient(string userAgent, ClientHints _)
    {
        return CombinedRegex.IsMatch(userAgent);
    }

    public override bool TryParse(
        string userAgent,
        ClientHints _,
        [NotNullWhen(true)] out ClientInfo? result
    )
    {
        result = null;

        if (TryParse(userAgent, FeedReaders, CombinedRegex, out var clientInfo))
        {
            result = new ClientInfo
            {
                Type = ClientType.FeedReader,
                Name = clientInfo.Name,
                Version = clientInfo.Version,
            };
        }

        return result is not null;
    }
}
