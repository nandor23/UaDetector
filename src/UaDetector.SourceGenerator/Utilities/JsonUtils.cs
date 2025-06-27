using System.Text.Json;
using UaDetector.SourceGenerator.Collections;
using UaDetector.SourceGenerator.Converters;

namespace UaDetector.SourceGenerator.Utilities;

internal static class JsonUtils
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new EquatableReadOnlyListJsonConverterFactory(),
            new EquatableReadOnlyDictionaryJsonConverterFactory(),
        },
    };

    public static EquatableReadOnlyList<T> DeserializeList<T>(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<EquatableReadOnlyList<T>>(json, SerializerOptions);
        }
        catch
        {
            return [];
        }
    }

    public static EquatableReadOnlyDictionary<string, string> DeserializeDictionary(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<EquatableReadOnlyDictionary<string, string>>(
                json,
                SerializerOptions
            );
        }
        catch
        {
            return [];
        }
    }
}
