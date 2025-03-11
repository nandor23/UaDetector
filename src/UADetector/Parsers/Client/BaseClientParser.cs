using System.Diagnostics.CodeAnalysis;

using UADetector.Regexes.Models.Client;
using UADetector.Results.Client;

namespace UADetector.Parsers.Client;

internal abstract class BaseClientParser<T> where T : IClient
{
    public abstract bool TryParse(
        string userAgent,
        ClientHints? clientHints,
        [NotNullWhen(true)] out IClientInfo? result
    );

    protected bool TryParse(
        string userAgent,
        IEnumerable<T> clients,
        [NotNullWhen(true)] out IClientInfo? result
    )
    {
        result = null;


        throw new NotImplementedException();
    }
}
