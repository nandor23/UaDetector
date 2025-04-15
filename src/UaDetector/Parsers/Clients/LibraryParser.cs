using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UaDetector.Models.Enums;
using UaDetector.Regexes.Models;
using UaDetector.Results;
using UaDetector.Utils;

namespace UaDetector.Parsers.Clients;

internal sealed class LibraryParser : ClientParserBase
{
    private const string ResourceName = "Regexes.Resources.Clients.libraries.json";
    private static readonly IEnumerable<Client> Libraries;
    private static readonly Regex CombinedRegex;


    static LibraryParser()
    {
        (Libraries, CombinedRegex) =
            RegexLoader.LoadRegexesWithCombined<Client>(ResourceName);
    }

    public LibraryParser(VersionTruncation versionTruncation) : base(versionTruncation)
    {
    }

    protected override ClientType Type => ClientType.Library;

    public override bool TryParse(string userAgent, ClientHints _, [NotNullWhen(true)] out ClientInfo? result)
    {
        return TryParse(userAgent, Libraries, CombinedRegex, out result);
    }
}
