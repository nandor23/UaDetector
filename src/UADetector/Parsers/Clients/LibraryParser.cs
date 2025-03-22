using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Models.Enums;
using UADetector.Regexes.Models;
using UADetector.Results;

namespace UADetector.Parsers.Clients;

internal sealed class LibraryParser : BaseClientParser
{
    private const string ResourceName = "Regexes.Resources.Clients.libraries.yml";
    private static readonly IEnumerable<Client> Libraries;
    private static readonly Regex OverallRegex;


    static LibraryParser()
    {
        (Libraries, OverallRegex) =
            ParserExtensions.LoadRegexesWithOverallRegex<Client>(ResourceName, RegexPatternType.UserAgent);
    }

    public LibraryParser(VersionTruncation versionTruncation) : base(versionTruncation)
    {
    }

    protected override ClientType Type => ClientType.Library;

    public override bool TryParse(string userAgent, ClientHints? _, [NotNullWhen(true)] out ClientInfo? result)
    {
        return TryParse(userAgent, Libraries, OverallRegex, out result);
    }
}
