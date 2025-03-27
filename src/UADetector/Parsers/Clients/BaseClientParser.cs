using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Models.Enums;
using UADetector.Regexes.Models;
using UADetector.Results;

namespace UADetector.Parsers.Clients;

internal abstract class BaseClientParser
{
    private readonly VersionTruncation _versionTruncation;

    protected abstract ClientType Type { get; }


    protected BaseClientParser(VersionTruncation versionTruncation)
    {
        _versionTruncation = versionTruncation;
    }

    public abstract bool TryParse(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out ClientInfo? result
    );

    protected bool TryParse(
        string userAgent,
        IEnumerable<Client> clients,
        Regex combinedRegex,
        [NotNullWhen(true)] out ClientInfo? result
    )
    {
        if (combinedRegex.IsMatch(userAgent))
        {
            foreach (var client in clients)
            {
                var match = client.Regex.Value.Match(userAgent);

                if (match.Success)
                {
                    result = new ClientInfo
                    {
                        Type = Type,
                        Name = ParserExtensions.FormatWithMatch(client.Name, match),
                        Version = ParserExtensions.BuildVersion(client.Version, match, _versionTruncation)
                    };

                    return true;
                }
            }
        }

        result = null;
        return false;
    }
}
