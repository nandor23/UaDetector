using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Models;
using UaDetector.Models.Enums;
using UaDetector.Results;

namespace UaDetector.Parsers.Clients;

internal abstract class ClientParserBase
{
    private readonly VersionTruncation _versionTruncation;

    protected ClientParserBase(VersionTruncation versionTruncation)
    {
        _versionTruncation = versionTruncation;
    }

    public abstract bool IsClient(string userAgent, ClientHints clientHints);

    public abstract bool TryParse(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out ClientInfo? result
    );

    protected bool TryParse(
        string userAgent,
        IReadOnlyList<RuleDefinition<Client>> clients,
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
                        Name = ParserExtensions.FormatWithMatch(client.Result.Name, match),
                        Version = ParserExtensions.BuildVersion(
                            client.Result.Version,
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
