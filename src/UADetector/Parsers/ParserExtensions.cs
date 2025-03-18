using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Models.Enums;
using UADetector.Utils;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UADetector.Parsers;

internal static class ParserExtensions
{
    private static readonly Regex ClientHintsFragmentMatchRegex = new(
        @"Android (?:10[.\d]*; K(?: Build/|[;)])|1[1-5]\)) AppleWebKit",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex ClientHintsFragmentReplacementRegex =
        new(@"Android (?:10[.\d]*; K|1[1-5])", RegexOptions.Compiled);

    private static readonly Regex DesktopFragmentMatchRegex =
        new("(?:Windows (?:NT|IoT)|X11; Linux x86_64)", RegexOptions.Compiled);

    private static readonly Regex DesktopFragmentReplacementRegex = new("X11; Linux x86_64", RegexOptions.Compiled);

    private static readonly Regex DesktopFragmentExclusionRegex = new(string.Join("|",
            "CE-HTML",
            " Mozilla/|Andr[o0]id|Tablet|Mobile|iPhone|Windows Phone|ricoh|OculusBrowser",
            "PicoBrowser|Lenovo|compatible; MSIE|Trident/|Tesla/|XBOX|FBMD/|ARM; ?([^)]+)"
        ),
        RegexOptions.Compiled
    );


    public static Regex BuildUserAgentRegex(string pattern)
    {
        return new Regex($"(?:^|[^A-Z0-9_-]|[^A-Z0-9-]_|sprd-|MZ-)(?:{pattern})",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    private static bool HasUserAgentClientHintsFragment(string userAgent)
    {
        return ClientHintsFragmentMatchRegex.IsMatch(userAgent);
    }

    private static bool HasUserAgentDesktopFragment(string userAgent)
    {
        return DesktopFragmentMatchRegex.IsMatch(userAgent) && !DesktopFragmentExclusionRegex.IsMatch(userAgent);
    }

    public static bool TryRestoreUserAgent(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out string? result
    )
    {
        result = null;

        if (string.IsNullOrEmpty(clientHints.Model))
        {
            return false;
        }

        if (HasUserAgentClientHintsFragment(userAgent))
        {
            var platformVersion =
                string.IsNullOrEmpty(clientHints.PlatformVersion) ? "10" : clientHints.PlatformVersion;

            result = ClientHintsFragmentReplacementRegex
                .Replace(userAgent, $"Android {platformVersion}; {clientHints.Model}");
        }

        if (HasUserAgentDesktopFragment(userAgent))
        {
            result = DesktopFragmentReplacementRegex
                .Replace(userAgent, $"X11; Linux x86_64; {clientHints.Model}");
        }

        return !string.IsNullOrEmpty(result);
    }

    public static IEnumerable<T> LoadRegexes<T>(
        string resourceName,
        RegexPatternType patternType = RegexPatternType.None
    )
    {
        var assembly = typeof(UADetector).Assembly;
        var fullResourceName = $"{nameof(UADetector)}.{resourceName}";

        using var stream = assembly.GetManifestResourceStream(fullResourceName);

        if (stream is null)
        {
            throw new InvalidOperationException(
                $"Embedded resource '{fullResourceName}' not found in assembly '{assembly.FullName}'.");
        }

        using var reader = new StreamReader(stream);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithTypeConverter(new YamlRegexConverter(patternType))
            .Build();

        return deserializer.Deserialize<IEnumerable<T>>(reader);
    }

    public static FrozenDictionary<string, string> LoadHints(string resourceName)
    {
        var assembly = typeof(UADetector).Assembly;
        var fullResourceName = $"{nameof(UADetector)}.{resourceName}";

        using var stream = assembly.GetManifestResourceStream(fullResourceName);

        if (stream is null)
        {
            throw new InvalidOperationException(
                $"Embedded resource '{fullResourceName}' not found in assembly '{assembly.FullName}'.");
        }

        using var reader = new StreamReader(stream);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        return deserializer.Deserialize<Dictionary<string, string>>(reader)
            .ToFrozenDictionary();
    }

    public static string FormatWithMatch(string value, Match match)
    {
        for (int i = 1; i < match.Groups.Count; i++)
        {
            value = value.Replace($"${i}", match.Groups[i].Value);
        }

        return value.Trim();
    }

    public static string? BuildVersion(string? version, VersionTruncation versionTruncation)
    {
        version = version?.Replace('_', '.');

        if (string.IsNullOrEmpty(version))
        {
            return null;
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

    public static string? BuildVersion(string? version, Match match, VersionTruncation versionTruncation)
    {
        if (string.IsNullOrEmpty(version))
        {
            return null;
        }

        version = FormatWithMatch(version, match);
        return BuildVersion(version, versionTruncation);
    }

    public static string NormalizeVersion(string version, string[] matches)
    {
        throw new NotImplementedException();
    }
}
