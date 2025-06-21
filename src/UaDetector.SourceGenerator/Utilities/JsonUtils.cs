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

    public static EquatableReadOnlyList<T> DeserializeJson<T>(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<EquatableReadOnlyList<T>>(json, SerializerOptions);
        }
        catch
        {
            // TODO: signal what went wrong
            return [];
        }
    }
}
