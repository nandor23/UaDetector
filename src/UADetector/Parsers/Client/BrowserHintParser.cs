using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

namespace UADetector.Parsers.Client;

internal static class BrowserHintParser
{
    private const string ResourceName = "Regexes.Resources.Client.Hints.browsers.yml";

    private static readonly FrozenDictionary<string, string> Hints = ParserExtensions.LoadHints(ResourceName);

    public static bool TryParse(ClientHints clientHints, [NotNullWhen(true)] out string? result)
    {
        if (string.IsNullOrEmpty(clientHints.App))
        {
            result = null;
        }
        else
        {
            Hints.TryGetValue(clientHints.App, out result);
        }

        return result is not null;
    }
}
