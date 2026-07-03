using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Abstractions.Enums;
using UaDetector.Utilities;

namespace UaDetector.Parsers;

internal static class ParserExtensions
{
    private static readonly Regex ClientHintsFragmentMatchRegex = new(
        @"Android (?:1[0-7][.\d]*; K(?: Build/|[;)])|1[0-7]\)) AppleWebKit",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    private static readonly Regex ClientHintsFragmentReplacementRegex = new(
        @"Android (?:10[.\d]*; K|1[1-7])",
        RegexOptions.Compiled
    );

    private static readonly Regex DesktopFragmentReplacementRegex = new(
        @"X11; Linux x86_64|Windows NT 10\.0; Win64; x64",
        RegexOptions.Compiled
    );

    private static readonly Regex DesktopFragmentMatchRegex = RegexBuilder.BuildRegex(
        "(?:Windows (?:NT|IoT)|X11; Linux x86_64)"
    );

    private static readonly Regex DesktopFragmentExclusionRegex = RegexBuilder.BuildRegex(
        string.Join(
            "|",
            "CE-HTML",
            " Mozilla/|Andr[o0]id|Tablet|Mobile|iPhone|Windows Phone|ricoh|OculusBrowser",
            "PicoBrowser|Lenovo|compatible; MSIE|Trident/|Tesla/|XBOX|FBMD/|ARM; ?([^)]+)"
        )
    );

    public static bool HasUserAgentClientHintsFragment(string userAgent)
    {
        if (!ClientHintsFragmentMatchRegex.IsMatch(userAgent))
        {
            return false;
        }

        return !userAgent.Contains("Telegram-Android/", StringComparison.OrdinalIgnoreCase);
    }

    public static bool HasUserAgentDesktopFragment(string userAgent)
    {
        return DesktopFragmentMatchRegex.IsMatch(userAgent)
            && !DesktopFragmentExclusionRegex.IsMatch(userAgent);
    }

    public static bool TryRestoreUserAgent(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out string? result
    )
    {
        result = null;

        if (clientHints.Model is null or { Length: 0 })
        {
            return false;
        }

        if (HasUserAgentClientHintsFragment(userAgent))
        {
            var platformVersion = clientHints.PlatformVersion is null or { Length: 0 }
                ? "10"
                : clientHints.PlatformVersion;

            result = ClientHintsFragmentReplacementRegex.Replace(
                userAgent,
                $"Android {platformVersion}; {clientHints.Model}"
            );
        }

        if (HasUserAgentDesktopFragment(userAgent))
        {
            result = DesktopFragmentReplacementRegex.Replace(
                userAgent,
                match => $"{match.Value}; {clientHints.Model}"
            );
        }

        return result?.Length > 0;
    }

    public static string FormatWithMatch(string value, Match match)
    {
        if (value.IndexOf('$') < 0)
        {
            return value.Trim();
        }

        for (int i = 1; i <= match.Groups.Count; i++)
        {
            value = value.Replace($"${i}", match.Groups[i].Value);
        }

        return value.Trim();
    }

    public static string? BuildVersion(string? version, VersionTruncation versionTruncation)
    {
        if (version is null or { Length: 0 })
        {
            return null;
        }

        if (version.IndexOf('_') >= 0)
        {
            version = version.Replace('_', '.');
        }

        if (versionTruncation != VersionTruncation.None)
        {
            var index = version.IndexOfNthOccurrence('.', (int)versionTruncation);

            if (index != -1)
            {
                version = version[..index];
            }
        }

        return version.Trim(' ', '.');
    }

    public static string? BuildVersion(
        string? version,
        Match match,
        VersionTruncation versionTruncation
    )
    {
        if (version is null or { Length: 0 })
        {
            return null;
        }

        version = FormatWithMatch(version, match);
        return BuildVersion(version, versionTruncation);
    }

    /// <summary>
    /// Extracts the leading numeric segment of <paramref name="version"/> as the major version.
    /// </summary>
    /// <param name="version">The version string to parse.</param>
    /// <returns>The major version, or 0 if <paramref name="version"/> is null, empty, or has no leading digits.</returns>
    public static int GetMajorVersion(string? version)
    {
        if (string.IsNullOrEmpty(version))
        {
            return 0;
        }

        var result = 0;

        foreach (var c in version)
        {
            if (c is < '0' or > '9')
            {
                break;
            }

            result = result * 10 + (c - '0');
        }

        return result;
    }
}
