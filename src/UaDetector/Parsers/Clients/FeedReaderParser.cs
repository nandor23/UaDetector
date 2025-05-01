using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UaDetector.Models.Enums;
using UaDetector.Regexes.Models;
using UaDetector.Results;
using UaDetector.Utils;

namespace UaDetector.Parsers.Clients;

internal sealed class FeedReaderParser : ClientParserBase
{
    private const string ResourceName = "Regexes.Resources.Clients.feed_readers.json";
    internal static readonly IReadOnlyList<Client> FeedReaders;
    private static readonly Regex CombinedRegex;

    static FeedReaderParser()
    {
        (FeedReaders, CombinedRegex) = RegexLoader.LoadRegexesWithCombined<Client>(ResourceName);
    }

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
