using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using UaDetector.SourceGenerator.Collections;
using UaDetector.SourceGenerator.Converters;

namespace UaDetector.SourceGenerator.Utilities;

public static class JsonUtils
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

    public static bool TryDeserializeList<T>(
        string json,
        [NotNullWhen(true)] out EquatableReadOnlyList<T>? result
    )
    {
        try
        {
            result = JsonSerializer.Deserialize<EquatableReadOnlyList<T>>(json, SerializerOptions);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public static bool TryDeserializeDictionary(
        string json,
        [NotNullWhen(true)] out EquatableReadOnlyDictionary<string, string>? result
    )
    {
        try
        {
            result = JsonSerializer.Deserialize<EquatableReadOnlyDictionary<string, string>>(
                json,
                SerializerOptions
            );

            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }
}
