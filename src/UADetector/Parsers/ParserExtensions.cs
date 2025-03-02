using System.Text.RegularExpressions;

using UADetector.Models.Enums;
using UADetector.Regexes.Models;
using UADetector.Utils;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UADetector.Parsers;

public static class ParserExtensions
{
    public static IEnumerable<T> LoadRegexes<T>(string resourceName,
        RegexPatternType patternType = RegexPatternType.None) where T : IRegexDefinition
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
