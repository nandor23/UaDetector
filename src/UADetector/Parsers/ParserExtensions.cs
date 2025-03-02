using UADetector.Regexes.Models;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UADetector.Parsers;

public static class ParserExtensions
{
    public static IEnumerable<T> LoadRegexes<T>(string resourceName,
        RegexPatternType patternType = RegexPatternType.None) where T : IRegexPattern
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

    public static void MatchUserAgent(string userAgent)
    {
        //        string regexPattern = @"(?:^|[^A-Z0-9_-]|[^A-Z0-9-]_|sprd-|MZ-)(?:" + Regex.Escape(innerRegex) + ")";

    }

    public static string NormalizeVersion(string version, string[] matches)
    {
        throw new NotImplementedException();
    }
}
