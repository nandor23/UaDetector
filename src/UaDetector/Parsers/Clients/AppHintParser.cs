using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

using UaDetector.Utils;

namespace UaDetector.Parsers.Clients;

internal static class AppHintParser
{
    private const string ResourceName = "Regexes.Resources.Clients.app_hints.json";
    private static readonly FrozenDictionary<string, string> Hints = RegexLoader.LoadHints(ResourceName);


    public static bool IsMobileApp(ClientHints clientHints)
    {
        return !string.IsNullOrEmpty(clientHints.App) && Hints.ContainsKey(clientHints.App);
    }
    
    public static bool TryParseAppName(ClientHints clientHints, [NotNullWhen(true)] out string? result)
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
