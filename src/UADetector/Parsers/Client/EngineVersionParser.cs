using System.Diagnostics.CodeAnalysis;

namespace UADetector.Parsers.Client;

internal sealed class EngineVersionParser
{
    public bool TryParse(string userAgent, string engine, [NotNullWhen(true)] out string? result)
    {
        if (string.IsNullOrEmpty(engine))
        {
            result = null;
            return false;
        }

        throw new NotImplementedException();
    }
}
