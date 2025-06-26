using System.Text.Json;
using System.Text.Json.Serialization;
using UaDetector.SourceGenerator.Collections;

namespace UaDetector.SourceGenerator.Converters;

/// <summary>
/// JSON converter for EquatableReadOnlyList.
/// </summary>
internal sealed class EquatableReadOnlyListJsonConverter<T>
    : JsonConverter<EquatableReadOnlyList<T>>
{
    public override EquatableReadOnlyList<T> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var list = JsonSerializer.Deserialize<List<T>>(ref reader, options);
        return new EquatableReadOnlyList<T>(list);
    }

    public override void Write(
        Utf8JsonWriter writer,
        EquatableReadOnlyList<T> value,
        JsonSerializerOptions options
    )
    {
        JsonSerializer.Serialize<IReadOnlyList<T>>(writer, value, options);
    }
}

/// <summary>
/// Generic converter factory for EquatableReadOnlyList.
/// </summary>
internal sealed class EquatableReadOnlyListJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType
               && typeToConvert.GetGenericTypeDefinition().FullName ==
               "UaDetector.SourceGenerator.Collections.EquatableReadOnlyList`1";
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var elementType = typeToConvert.GetGenericArguments()[0];

        var converterType = typeof(EquatableReadOnlyListJsonConverter<>).MakeGenericType(
            elementType
        );

        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}
