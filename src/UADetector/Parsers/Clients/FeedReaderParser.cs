using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Models.Enums;
using UADetector.Regexes.Models;
using UADetector.Results;

namespace UADetector.Parsers.Clients;

internal sealed class FeedReaderParser : BaseClientParser
{
    private const string ResourceName = "Regexes.Resources.Clients.feed_readers.yml";
    private static readonly IEnumerable<Client> FeedReaders;
    private static readonly Regex CombinedRegex;


    static FeedReaderParser()
    {
        (FeedReaders, CombinedRegex) =
            ParserExtensions.LoadRegexes<Client>(ResourceName);
    }

    public FeedReaderParser(VersionTruncation versionTruncation) : base(versionTruncation)
    {
    }

    protected override ClientType Type => ClientType.FeedReader;

    public override bool TryParse(string userAgent, ClientHints _, [NotNullWhen(true)] out ClientInfo? result)
    {
        return TryParse(userAgent, FeedReaders, CombinedRegex, out result);
    }
}
