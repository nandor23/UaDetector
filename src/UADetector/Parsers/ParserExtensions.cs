using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Models.Enums;
using UADetector.Utils;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UADetector.Parsers;

internal static partial class ParserExtensions
{
    private const string ClientHintsFragmentMatchPattern = @"Android (?:10[.\d]*; K(?: Build/|[;)])|1[1-5]\)) AppleWebKit";
    private const string ClientHintsFragmentReplacementPattern = @"Android (?:10[\.\d]*; K|1[1-5])";
    private const string DesktopFragmentMatchPattern = "(?:Windows (?:NT|IoT)|X11; Linux x86_64)";
    private const string DesktopFragmentReplacementPattern = "X11; Linux x86_64";

    private const string DesktopFragmentExclusionPattern =
        "CE-HTML|" +
        "Mozilla/|Andr[o0]id|Tablet|Mobile|iPhone|Windows Phone|ricoh|OculusBrowser|" +
        "PicoBrowser|Lenovo|compatible; MSIE|Trident/|Tesla/|XBOX|FBMD/|ARM; ?([^)]+)";

#if NET7_0_OR_GREATER
    [GeneratedRegex(ClientHintsFragmentMatchPattern, RegexOptions.IgnoreCase)]
    private static partial Regex ClientHintsFragmentMatchRegex();
    
    [GeneratedRegex(ClientHintsFragmentReplacementPattern)]
    private static partial Regex ClientHintsFragmentReplacementRegex();

    [GeneratedRegex(DesktopFragmentMatchPattern)]
    private static partial Regex DesktopFragmentMatchRegex();
    
    [GeneratedRegex(DesktopFragmentReplacementPattern)]
    private static partial Regex DesktopFragmentReplacementRegex();
    
    [GeneratedRegex(DesktopFragmentExclusionPattern)]
    private static partial Regex DesktopFragmentExclusionRegex();
#else
    private static readonly Regex ClientHintsFragmentMatchInstance =
        new(ClientHintsFragmentMatchPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex ClientHintsFragmentReplacementInstance =
        new(ClientHintsFragmentReplacementPattern, RegexOptions.Compiled);

    private static readonly Regex DesktopFragmentMatchInstance =
        new(DesktopFragmentMatchPattern, RegexOptions.Compiled);

    private static readonly Regex DesktopFragmentReplacementInstance =
        new(DesktopFragmentReplacementPattern, RegexOptions.Compiled);

    private static readonly Regex DesktopFragmentExclusionInstance =
        new(DesktopFragmentExclusionPattern, RegexOptions.Compiled);


    private static Regex ClientHintsFragmentMatchRegex() => ClientHintsFragmentMatchInstance;
    private static Regex ClientHintsFragmentReplacementRegex() => ClientHintsFragmentReplacementInstance;
    private static Regex DesktopFragmentMatchRegex() => DesktopFragmentMatchInstance;
    private static Regex DesktopFragmentReplacementRegex() => DesktopFragmentReplacementInstance;
    private static Regex DesktopFragmentExclusionRegex() => DesktopFragmentExclusionInstance;
#endif


    public static Regex BuildUserAgentRegex(string pattern)
    {
        return new Regex($"(?:^|[^A-Z0-9_-]|[^A-Z0-9-]_|sprd-|MZ-)(?:{pattern})",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    private static bool HasUserAgentClientHintsFragment(string userAgent)
    {
        return ClientHintsFragmentMatchRegex().IsMatch(userAgent);
    }

    private static bool HasUserAgentDesktopFragment(string userAgent)
    {
        return DesktopFragmentMatchRegex().IsMatch(userAgent) && !DesktopFragmentExclusionRegex().IsMatch(userAgent);
    }

    public static bool TryRestoreUserAgent(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out string? result
    )
    {
        result = null;

        if (clientHints.Model is null)
        {
            return false;
        }

        if (HasUserAgentClientHintsFragment(userAgent))
        {
            var platformVersion =
                string.IsNullOrEmpty(clientHints.PlatformVersion) ? "10" : clientHints.PlatformVersion;

            result = ClientHintsFragmentReplacementRegex()
                .Replace(userAgent, $"Android {platformVersion}; {clientHints.Model}");
        }

        if (HasUserAgentDesktopFragment(userAgent))
        {
            result = DesktopFragmentReplacementRegex()
                .Replace(userAgent, $"{DesktopFragmentReplacementPattern}; {clientHints.Model}");
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

    public static string? FormatWithMatch(string? value, Match match)
    {
        if (value is null)
        {
            return null;
        }

        for (int i = 1; i < match.Groups.Count; i++)
        {
            value = value.Replace($"${i}", match.Groups[i].Value);
        }

        return value.Trim();
    }

    public static string? FormatVersionWithMatch(string? version, Match match, VersionTruncation versionTruncation)
    {
        if (version is null)
        {
            return null;
        }

        version = FormatWithMatch(version, match)?.Replace('_', '.');

        if (versionTruncation != VersionTruncation.None && version is not null)
        {
            var index = version.IndexOfNthOccurrence('.', (int)versionTruncation);

            if (index != -1)
            {
                version = version[..index];
            }
        }

        return version?.Trim(' ', '.');
    }

    public static string NormalizeVersion(string version, string[] matches)
    {
        throw new NotImplementedException();
    }
}
