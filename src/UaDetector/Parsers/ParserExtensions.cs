using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Abstractions.Enums;
using UaDetector.Utilities;

namespace UaDetector.Parsers;

internal static class ParserExtensions
{
    private static readonly Regex ClientHintsFragmentMatchRegex = new(
        @"Android (?:1[0-6][.\d]*; K(?: Build/|[;)])|1[0-6]\)) AppleWebKit",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    private static readonly Regex ClientHintsFragmentReplacementRegex = new(
        @"Android (?:10[.\d]*; K|1[1-5])",
        RegexOptions.Compiled
    );

    private static readonly Regex DesktopFragmentReplacementRegex = new(
        "X11; Linux x86_64",
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
                $"X11; Linux x86_64; {clientHints.Model}"
            );
        }

        return result?.Length > 0;
    }

    public static string FormatWithMatch(string value, Match match)
    {
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

        version = version.Replace('_', '.');

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
    /// Tries to compare <paramref name="first"/> and <paramref name="second"/>.
    /// </summary>
    /// <param name="first">The first version string to compare.</param>
    /// <param name="second">The second version string to compare.</param>
    /// <param name="result">
    /// The comparison result:
    /// - Less than zero if <paramref name="first"/> is less than <paramref name="second"/>.
    /// - Zero if they are equal.
    /// - Greater than zero if <paramref name="first"/> is greater than <paramref name="second"/>.
    /// Only set if the comparison succeeds.
    /// </param>
    /// <returns>
    /// True if the comparison was successful, false otherwise.
    /// </returns>
    public static bool TryCompareVersions(
        string first,
        string second,
        [NotNullWhen(true)] out int? result
    )
    {
        string[] segments1 = first.Split('.');
        string[] segments2 = second.Split('.');

        int maxSegments = Math.Max(segments1.Length, segments2.Length);

        for (int i = 0; i < maxSegments; i++)
        {
            int value1,
                value2;

            if (i < segments1.Length)
            {
                if (!int.TryParse(segments1[i], out value1))
                {
                    result = null;
                    return false;
                }
            }
            else
            {
                value1 = 0;
            }

            if (i < segments2.Length)
            {
                if (!int.TryParse(segments2[i], out value2))
                {
                    result = null;
                    return false;
                }
            }
            else
            {
                value2 = 0;
            }

            result = value1.CompareTo(value2);

            if (result != 0)
            {
                return true;
            }
        }

        result = 0;
        return true;
    }
}
