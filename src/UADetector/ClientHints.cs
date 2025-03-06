using System.Collections.Frozen;
using System.Text.RegularExpressions;

namespace UADetector;

public sealed partial class ClientHints
{
    private const string FullVersionListPattern = """^"([^"]+)"; ?v="([^"]+)"(?:, )?""";
    private const string FormFactorsPattern = """
                                              "([a-zA-Z]+)"
                                              """;


#if NET7_0_OR_GREATER
    [GeneratedRegex(FullVersionListPattern)]
    private static partial Regex FullVersionListRegex();
    
    [GeneratedRegex(FormFactorsPattern)]
    private static partial Regex FormFactorsRegex();
#else
    private static readonly Regex FullVersionListRegexInstance = new(FullVersionListPattern, RegexOptions.Compiled);
    private static readonly Regex FormFactorsRegexInstance = new(FormFactorsPattern, RegexOptions.Compiled);

    private static Regex FullVersionListRegex() => FullVersionListRegexInstance;
    private static Regex FormFactorsRegex() => FormFactorsRegexInstance;
#endif


    private static readonly FrozenSet<string> ArchitectureHeaderNames =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "http-sec-ch-ua-arch", "sec-ch-ua-arch", "arch", "architecture"
            }
            .ToFrozenSet();

    private static readonly FrozenSet<string> BitnessHeaderNames =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "http-sec-ch-ua-bitness", "sec-ch-ua-bitness", "bitness"
            }
            .ToFrozenSet();

    private static readonly FrozenSet<string> MobileHeaderNames =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "http-sec-ch-ua-mobile", "sec-ch-ua-mobile", "mobile" }
            .ToFrozenSet();

    private static readonly FrozenSet<string> ModelHeaderNames =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "http-sec-ch-ua-model", "sec-ch-ua-model", "model" }
            .ToFrozenSet();

    private static readonly FrozenSet<string> PlatformHeaderNames =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "http-sec-ch-ua-platform", "sec-ch-ua-platform", "platform"
            }
            .ToFrozenSet();

    private static readonly FrozenSet<string> PlatformVersionHeaderNames =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "http-sec-ch-ua-platform-version", "sec-ch-ua-platform-version", "platformversion"
            }
            .ToFrozenSet();

    private static readonly FrozenSet<string> UaFullVersionHeaderNames =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "http-sec-ch-ua-full-version", "sec-ch-ua-full-version", "uafullversion"
            }
            .ToFrozenSet();

    private static readonly FrozenSet<string> PrimaryFullVersionListHeaderNames =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "fullversionlist", "http-sec-ch-ua-full-version-list", "sec-ch-ua-full-version-list"
            }
            .ToFrozenSet();

    private static readonly FrozenSet<string> SecondaryFullVersionListHeaderNames =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "brands", "http-sec-ch-ua", "sec-ch-ua" }
            .ToFrozenSet();

    private static readonly FrozenSet<string> AppHeaderNames =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "http-x-requested-with", "x-requested-with" }
            .ToFrozenSet();

    private static readonly FrozenSet<string> FormFactorsHeaderNames =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "formfactors", "http-sec-ch-ua-form-factors", "sec-ch-ua-form-factors"
            }
            .ToFrozenSet();


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
    public Dictionary<string, string>? FullVersionList { get; } = new();

    /// <summary>
    /// Represents <c>x-requested-with</c> header field: Android app id
    /// </summary>
    public string? App { get; private set; }

    /// <summary>
    /// Represents <c>Sec-CH-UA-Form-Factors</c> header field: form factor device type name
    /// </summary>
    public List<string> FormFactors { get; private set; } = [];


    /// <summary>
    /// Create a new ClientHints instance from a dictionary containing all available client hint headers.
    /// </summary>
    public static ClientHints Create(IDictionary<string, string> headers)
    {
        var clientHints = new ClientHints();

        foreach (var header in headers.Where(h => !string.IsNullOrEmpty(h.Value)))
        {
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
                      clientHints.FullVersionList?.Count == 0))
            {
                var regex = FullVersionListRegex();

                foreach (Match match in regex.Matches(normalizedHeader))
                {
                    clientHints.FullVersionList?.Add(match.Groups[1].Value, match.Groups[2].Value);
                }
            }
            else if (AppHeaderNames.Contains(normalizedHeader) &&
                     value.Equals("xmlhttprequest", StringComparison.OrdinalIgnoreCase))
            {
                clientHints.App = value;
            }
            else if (FormFactorsHeaderNames.Contains(normalizedHeader))
            {
                var formFactors = value.Split(',').ToList();

                if (formFactors.Count > 1)
                {
                    clientHints.FormFactors = formFactors;
                }
                else
                {
                    var regex = FormFactorsRegex();

                    foreach (Match match in regex.Matches(normalizedHeader))
                    {
                        clientHints.FormFactors.Add(match.Value);
                    }
                }
            }
        }

        return clientHints;
    }
}
