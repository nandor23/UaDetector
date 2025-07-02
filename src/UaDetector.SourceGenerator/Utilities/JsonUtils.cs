using System.Text.Json;
using Microsoft.CodeAnalysis;
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
        catch (Exception)
        {
            return [];
        }
    }

    public static EquatableReadOnlyDictionary<string, string> DeserializeDictionary(
        string json,
        SourceProductionContext context
    )
    {
        try
        {
            return JsonSerializer.Deserialize<EquatableReadOnlyDictionary<string, string>>(
                json,
                SerializerOptions
            );
        }
        catch (Exception)
        {
            string jsonSnippet = json.Length > 100 ? json[..100] + "..." : json;

            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.JsonDeserializeFailed,
                Location.None,
                jsonSnippet
            );

            context.ReportDiagnostic(diagnostic);

            return [];
        }
    }
}
