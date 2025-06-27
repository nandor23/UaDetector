using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using UaDetector.SourceGenerator.Collections;

namespace UaDetector.SourceGenerator.Converters;

/// <summary>
/// JSON converter for EquatableReadOnlyDictionary.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class EquatableReadOnlyDictionaryJsonConverter<TKey, TValue>
    : JsonConverter<EquatableReadOnlyDictionary<TKey, TValue>>
    where TKey : notnull
{
    public override EquatableReadOnlyDictionary<TKey, TValue> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var dictionary = JsonSerializer.Deserialize<Dictionary<TKey, TValue>>(ref reader, options);
        return new EquatableReadOnlyDictionary<TKey, TValue>(dictionary);
    }

    public override void Write(
        Utf8JsonWriter writer,
        EquatableReadOnlyDictionary<TKey, TValue> value,
        JsonSerializerOptions options
    )
    {
        JsonSerializer.Serialize<IReadOnlyDictionary<TKey, TValue>>(writer, value, options);
    }
}

/// <summary>
/// Generic converter factory for EquatableReadOnlyDictionary.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class EquatableReadOnlyDictionaryJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType
            && typeToConvert.GetGenericTypeDefinition().FullName
                == "UaDetector.SourceGenerator.Collections.EquatableReadOnlyDictionary`2";
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var keyType = typeToConvert.GetGenericArguments()[0];
        var valueType = typeToConvert.GetGenericArguments()[1];

        var converterType = typeof(EquatableReadOnlyDictionaryJsonConverter<,>).MakeGenericType(
            keyType,
            valueType
        );

        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}
