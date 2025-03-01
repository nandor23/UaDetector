using UADetector.Regexes.Models;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UADetector.Parsers;

public static class ParserExtensions
{
    private static readonly IDeserializer Deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    public static IEnumerable<T> LoadRegexes<T>(string resourceName) where T : IRegexPattern
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
        return Deserializer.Deserialize<IEnumerable<T>>(reader);
    }

    public static string NormalizeVersion(string version, string[] matches)
    {
        throw new NotImplementedException();
    }
}
