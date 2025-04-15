using System.Collections.Frozen;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace UaDetector.Utils;

internal static class RegexLoader
{
    private static Stream GetEmbeddedResourceStream(string resourceName)
    {
        var assembly = typeof(UaDetector).Assembly;
        var fullResourceName = $"{nameof(UaDetector)}.{resourceName}";

        var stream = assembly.GetManifestResourceStream(fullResourceName);

        if (stream is null)
        {
            throw new InvalidOperationException(
                $"Embedded resource '{fullResourceName}' not found in assembly '{assembly.FullName}'.");
        }

        return stream;
    }

    private static JsonSerializerOptions CreateSerializerOptions(RegexJsonConverter regexConverter)
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            RespectRequiredConstructorParameters = true,
            Converters = { regexConverter }
        };
    }

    public static IEnumerable<T> LoadRegexes<T>(string resourceName, string? patternSuffix = null)
    {
        var regexConverter = new RegexJsonConverter(patternSuffix);
        var serializerOptions = CreateSerializerOptions(regexConverter);
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

    public static FrozenDictionary<string, string> LoadHints(string resourceName)
    {
        var regexConverter = new RegexJsonConverter();
        var serializerOptions = CreateSerializerOptions(regexConverter);
        using var stream = GetEmbeddedResourceStream(resourceName);
        using var reader = new StreamReader(stream);

        var hints = JsonSerializer.Deserialize<Dictionary<string, string>>(stream, serializerOptions) ?? [];

        return hints.ToFrozenDictionary();
    }
}
