using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

using UaDetector.Attributes;

namespace UaDetector.Parsers.Clients;

internal static partial class AppHintParser
{
    [HintSource("Resources/Clients/app_hints.json")]
    internal static partial FrozenDictionary<string, string> Hints { get; }

    public static bool IsMobileApp(ClientHints clientHints)
    {
        return clientHints.App?.Length > 0 && Hints.ContainsKey(clientHints.App);
    }

    public static bool TryParseAppName(
        ClientHints clientHints,
        [NotNullWhen(true)] out string? result
    )
    {
        if (clientHints.App is null or { Length: 0 })
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
