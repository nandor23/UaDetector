using System.Text.Json;
using UaDetector.SourceGenerator.Collections;

namespace UaDetector.SourceGenerator.Utilities;

internal static class JsonUtils
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static EquatableReadOnlyList<T> DeserializeJson<T>(string json)
    {
        try
        {
            var list = JsonSerializer
                .Deserialize<List<T>>(json, SerializerOptions)
                ?.ToEquatableReadOnlyList();

            return list ?? [];
        }
        catch
        {
            // TODO: signal what went wrong
            return [];
        }
    }
}
