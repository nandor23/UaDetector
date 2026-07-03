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
        ReadOnlySpan<char> firstSpan = first.AsSpan();
        ReadOnlySpan<char> secondSpan = second.AsSpan();

        int offset1 = 0,
            offset2 = 0;

        bool hasSegment1 = true,
            hasSegment2 = true;

        while (hasSegment1 || hasSegment2)
        {
            int value1 = 0,
                value2 = 0;

            if (hasSegment1)
            {
                var segment = NextVersionSegment(firstSpan, ref offset1, out hasSegment1);

                if (hasSegment1 && !TryParseVersionSegment(segment, out value1))
                {
                    result = null;
                    return false;
                }
            }

            if (hasSegment2)
            {
                var segment = NextVersionSegment(secondSpan, ref offset2, out hasSegment2);

                if (hasSegment2 && !TryParseVersionSegment(segment, out value2))
                {
                    result = null;
                    return false;
                }
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

    /// <summary>
    /// Returns the next '.'-delimited segment starting at <paramref name="offset"/>, mirroring
    /// <see cref="string.Split(char[])"/> semantics without allocating. <paramref name="hasSegment"/>
    /// is set to false once the string has been fully consumed.
    /// </summary>
    private static ReadOnlySpan<char> NextVersionSegment(
        ReadOnlySpan<char> text,
        ref int offset,
        out bool hasSegment
    )
    {
        if (offset > text.Length)
        {
            hasSegment = false;
            return default;
        }

        hasSegment = true;
        var remaining = text[offset..];
        int dotIndex = remaining.IndexOf('.');

        if (dotIndex < 0)
        {
            offset = text.Length + 1;
            return remaining;
        }

        offset += dotIndex + 1;
        return remaining[..dotIndex];
    }

    private static bool TryParseVersionSegment(ReadOnlySpan<char> segment, out int value)
    {
#if NET6_0_OR_GREATER
        return int.TryParse(segment, out value);
#else
        return int.TryParse(segment.ToString(), out value);
#endif
    }
}
