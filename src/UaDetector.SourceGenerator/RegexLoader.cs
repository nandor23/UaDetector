using System.Text.Json;

namespace UaDetector.SourceGenerator;

internal static class RegexLoader
{
    private static JsonSerializerOptions CreateSerializerOptions(RegexJsonConverter regexConverter)
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { regexConverter },
        };
    }

    /*public static IReadOnlyList<T> LoadRegexes<T>(string resourceName, string? patternSuffix = null)
    {
        var regexConverter = new RegexJsonConverter(patternSuffix);
        var serializerOptions = CreateSerializerOptions(regexConverter);

        return JsonSerializer.Deserialize<List<T>>(stream, serializerOptions) ?? [];
    }*/
}
