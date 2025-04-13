using System.Collections.Frozen;
using System.Text.Json;
using System.Text.RegularExpressions;

using UADetector.Parsers;

namespace UADetector.Utils;

internal static class RegexLoader
{
    private static Stream GetEmbeddedResourceStream(string resourceName)
    {
        var assembly = typeof(UADetector).Assembly;
        var fullResourceName = $"{nameof(UADetector)}.{resourceName}";

        var stream = assembly.GetManifestResourceStream(fullResourceName);

        if (stream is null)
        {
            throw new InvalidOperationException(
                $"Embedded resource '{fullResourceName}' not found in assembly '{assembly.FullName}'.");
        }

        return stream;
    }

    private static JsonSerializerOptions CreateSerializerOptions(RegexJsonConverter? regexConverter = null)
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            RespectRequiredConstructorParameters = true,
            Converters = { regexConverter ?? new RegexJsonConverter() }
        };
    }

    public static IEnumerable<T> LoadRegexes<T>(string resourceName)
    {
        var serializerOptions = CreateSerializerOptions();
        using var stream = GetEmbeddedResourceStream(resourceName);
        using var reader = new StreamReader(stream);

        return JsonSerializer.Deserialize<IEnumerable<T>>(stream, serializerOptions) ?? [];
    }

    public static (IEnumerable<T>, Regex) LoadRegexesWithCombined<T>(string resourceName)
    {
        var regexConverter = new RegexJsonConverter();
        var serializerOptions = CreateSerializerOptions(regexConverter);
        using var stream = GetEmbeddedResourceStream(resourceName);
        using var reader = new StreamReader(stream);

        var regexes = JsonSerializer.Deserialize<IEnumerable<T>>(stream, serializerOptions);
        var combinedRegex = regexConverter.BuildCombinedRegex();

        return (regexes ?? [], combinedRegex);
    }

    public static (FrozenDictionary<string, T>, Regex) LoadRegexesDictionaryWithCombined<T>(
        string resourceName,
        string? patternSuffix = null
    )
    {
        var regexConverter = new RegexJsonConverter(patternSuffix);
        var serializerOptions = CreateSerializerOptions(regexConverter);
        using var stream = GetEmbeddedResourceStream(resourceName);
        using var reader = new StreamReader(stream);


        var regexes = JsonSerializer.Deserialize<Dictionary<string, T>>(stream, serializerOptions);
        var combinedRegex = regexConverter.BuildCombinedRegex();

        return ((regexes ?? []).ToFrozenDictionary(), combinedRegex);
    }

    public static FrozenDictionary<string, string> LoadHints(string resourceName)
    {
        var serializerOptions = CreateSerializerOptions();
        using var stream = GetEmbeddedResourceStream(resourceName);
        using var reader = new StreamReader(stream);

        var hints = JsonSerializer.Deserialize<Dictionary<string, string>>(stream, serializerOptions) ?? [];

        return hints.ToFrozenDictionary();
    }
}
