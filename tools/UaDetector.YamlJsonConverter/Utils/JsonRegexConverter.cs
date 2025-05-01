using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace UaDetector.YamlJsonConverter.Utils;

public sealed class RegexJsonConverter : JsonConverter<Regex>
{
    public override Regex Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        string? pattern = reader.GetString();

        if (pattern is null)
        {
            throw new JsonException("Regex pattern cannot be null");
        }

        return new Regex(pattern);
    }

    public override void Write(Utf8JsonWriter writer, Regex value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
