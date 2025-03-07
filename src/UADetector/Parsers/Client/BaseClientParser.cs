using System.Diagnostics.CodeAnalysis;

using UADetector.Regexes.Models.Client;
using UADetector.Results;

namespace UADetector.Parsers.Client;

internal abstract class BaseClientParser<T> where T : IClientDefinition
{
    public abstract bool TryParse(string userAgent, [NotNullWhen(true)] out ClientInfo? result);

    protected bool TryParse(
        string userAgent,
        IEnumerable<T> clientDefinitions,
        [NotNullWhen(true)] out ClientInfo? result
    )
    {
        result = null;


        throw new NotImplementedException();
    }
}
