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
    private static readonly Regex OverallRegex;
    

    static FeedReaderParser()
    {
        (FeedReaders, OverallRegex) =
            ParserExtensions.LoadRegexesWithOverallRegex<Client>(ResourceName, RegexPatternType.UserAgent);
    }

    public FeedReaderParser(VersionTruncation versionTruncation) : base(versionTruncation)
    {
    }

    public override bool TryParse(string userAgent, [NotNullWhen(true)] out ClientInfo? result)
    {
        if (TryParse(userAgent, FeedReaders, OverallRegex, out result))
        {
            result.Type = ClientType.FeedReader;
        }

        return result is not null;
    }
}
