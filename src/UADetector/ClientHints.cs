using System.Collections.Frozen;
using System.Text.RegularExpressions;

namespace UADetector;

internal sealed class ClientHints
{
    private static readonly Regex FullVersionListRegex =
        new("""^"([^"]+)"; ?v="([^"]+)"(?:, )?""", RegexOptions.Compiled);

    private static readonly Regex FormFactorsRegex = new("""
                                                         "([a-zA-Z]+)"
                                                         """, RegexOptions.Compiled);

    internal static readonly FrozenSet<string> ArchitectureHeaderNames =
        new HashSet<string>
            {
                "http-sec-ch-ua-arch", "sec-ch-ua-arch", "arch", "architecture"
            }
            .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    internal static readonly FrozenSet<string> BitnessHeaderNames =
        new HashSet<string>
            {
                "http-sec-ch-ua-bitness", "sec-ch-ua-bitness", "bitness"
            }
            .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    internal static readonly FrozenSet<string> MobileHeaderNames =
        new HashSet<string> { "http-sec-ch-ua-mobile", "sec-ch-ua-mobile", "mobile" }
            .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    internal static readonly FrozenSet<string> ModelHeaderNames =
        new HashSet<string> { "http-sec-ch-ua-model", "sec-ch-ua-model", "model" }
            .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    internal static readonly FrozenSet<string> PlatformHeaderNames =
        new HashSet<string>
            {
                "http-sec-ch-ua-platform", "sec-ch-ua-platform", "platform"
            }
            .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    internal static readonly FrozenSet<string> PlatformVersionHeaderNames =
        new HashSet<string>
            {
                "http-sec-ch-ua-platform-version", "sec-ch-ua-platform-version", "platformversion"
            }
            .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    internal static readonly FrozenSet<string> UaFullVersionHeaderNames =
        new HashSet<string>
            {
                "http-sec-ch-ua-full-version", "sec-ch-ua-full-version", "uafullversion"
            }
            .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    internal static readonly FrozenSet<string> PrimaryFullVersionListHeaderNames =
        new HashSet<string> { "http-sec-ch-ua-full-version-list", "sec-ch-ua-full-version-list" }
            .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    internal static readonly FrozenSet<string> SecondaryFullVersionListHeaderNames =
        new HashSet<string> { "http-sec-ch-ua", "sec-ch-ua" }
            .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    internal static readonly FrozenSet<string> AppHeaderNames =
        new HashSet<string> { "http-x-requested-with", "x-requested-with" }
            .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    internal static readonly FrozenSet<string> FormFactorsHeaderNames =
        new HashSet<string>
            {
                "formfactors", "http-sec-ch-ua-form-factors", "sec-ch-ua-form-factors"
            }
            .ToFrozenSet(StringComparer.OrdinalIgnoreCase);


    /// <summary>
    /// Represents <c>Sec-CH-UA-Arch</c> header field: The underlying architecture's instruction set
    /// </summary>
    public string? Architecture { get; private set; }

    /// <summary>
    /// Represents <c>Sec-CH-UA-Bitness</c> header field: The underlying architecture's bitness
    /// </summary>
    public string? Bitness { get; private set; }

    /// <summary>
    /// Represents <c>Sec-CH-UA-Mobile</c> header field: whether the user agent should receive a specifically "mobile" UX
    /// </summary>
    public bool IsMobile { get; private set; }

    /// <summary>
    /// Represents <c>Sec-CH-UA-Model</c> header field: the user agent's underlying device model
    /// </summary>
    public string? Model { get; private set; }

    /// <summary>
    /// Represents <c>Sec-CH-UA-Platform</c> header field: the platform's brand
    /// </summary>
    public string? Platform { get; private set; }

    /// <summary>
    /// Represents <c>Sec-CH-UA-Platform-Version</c> header field: the platform's major version
    /// </summary>
    public string? PlatformVersion { get; private set; }

    /// <summary>
    /// Represents <c>Sec-CH-UA-Full-Version</c> header field: the platform's major version
    /// </summary>
    public string? UaFullVersion { get; private set; }

    /// <summary>
    /// Represents <c>Sec-CH-UA-Full-Version-List</c> header field: the full version for each brand in its brand list
    /// </summary>
    public Dictionary<string, string> FullVersionList { get; } = [];

    /// <summary>
    /// Represents <c>x-requested-with</c> header field: Android app id
    /// </summary>
    public string? App { get; private set; }

    /// <summary>
    /// Represents <c>Sec-CH-UA-Form-Factors</c> header field: form factor device type name
    /// </summary>
    public HashSet<string> FormFactors { get; } = new(StringComparer.OrdinalIgnoreCase);


    /// <summary>
    /// Create a new ClientHints instance from a dictionary containing all available client hint headers.
    /// </summary>
    public static ClientHints Create(IDictionary<string, string?> headers)
    {
        ClientHints clientHints = new();

        foreach (var header in headers)
        {
            if (string.IsNullOrEmpty(header.Value))
            {
                continue;
            }

            var normalizedHeader = header.Key.Replace('_', '-');
            var value = header.Value;

            if (ArchitectureHeaderNames.Contains(normalizedHeader))
            {
                clientHints.Architecture = value.Trim('"');
            }
            else if (BitnessHeaderNames.Contains(normalizedHeader))
            {
                clientHints.Bitness = value.Trim('"');
            }
            else if (MobileHeaderNames.Contains(normalizedHeader))
            {
                clientHints.IsMobile = value is "1" or "?1";
            }
            else if (ModelHeaderNames.Contains(normalizedHeader))
            {
                clientHints.Model = value.Trim('"');
            }
            else if (UaFullVersionHeaderNames.Contains(normalizedHeader))
            {
                clientHints.UaFullVersion = value.Trim('"');
            }
            else if (PlatformHeaderNames.Contains(normalizedHeader))
            {
                clientHints.Platform = value.Trim('"');
            }
            else if (PlatformVersionHeaderNames.Contains(normalizedHeader))
            {
                clientHints.PlatformVersion = value.Trim('"');
            }
            else if (PrimaryFullVersionListHeaderNames.Contains(normalizedHeader) ||
                     (SecondaryFullVersionListHeaderNames.Contains(normalizedHeader) &&
                      clientHints.FullVersionList.Count == 0))
            {
                while (FullVersionListRegex.Match(value) is { Success: true } match)
                {
                    if (match.Groups.Count == 3 && !clientHints.FullVersionList.ContainsKey(match.Groups[1].Value))
                    {
                        clientHints.FullVersionList.Add(match.Groups[1].Value, match.Groups[2].Value);
                    }

                    value = value[match.Length..];
                }
            }
            else if (AppHeaderNames.Contains(normalizedHeader) &&
                     !value.Equals("xmlhttprequest", StringComparison.OrdinalIgnoreCase))
            {
                clientHints.App = value;
            }
            else if (FormFactorsHeaderNames.Contains(normalizedHeader))
            {
                var match = FormFactorsRegex.Match(value);

                while (match is { Success: true })
                {
                    clientHints.FormFactors.Add(match.Groups[1].Value);
                    match = match.NextMatch();
                }
            }
        }

        return clientHints;
    }
}
