using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Models.Enums;
using UaDetector.Regexes.Models;
using UaDetector.Results;

namespace UaDetector.Parsers.Clients;

internal abstract class ClientParserBase
{
    private readonly VersionTruncation _versionTruncation;
    private readonly ParserHelper _parserHelper;

    protected ClientParserBase(VersionTruncation versionTruncation)
    {
        _versionTruncation = versionTruncation;
        _parserHelper = new ParserHelper();
    }

    public abstract bool IsClient(string userAgent, ClientHints clientHints);

    public abstract bool TryParse(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out ClientInfo? result
    );

    protected bool TryParse(
        string userAgent,
        IEnumerable<Client> clients,
        Regex combinedRegex,
        [NotNullWhen(true)] out ClientInfoInternal? result
    )
    {
        if (combinedRegex.IsMatch(userAgent))
        {
            foreach (var client in clients)
            {
                var match = client.Regex.Match(userAgent);

                if (match.Success)
                {
                    result = new ClientInfoInternal
                    {
                        Name = _parserHelper.FormatWithMatch(client.Name, match),
                        Version = _parserHelper.BuildVersion(
                            client.Version,
                            match,
                            _versionTruncation
                        ),
                    };

                    return true;
                }
            }
        }

        result = null;
        return false;
    }
}
