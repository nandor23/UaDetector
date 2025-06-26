using System.Collections.Frozen;
using System.Text.Json;

namespace UaDetector.Utilities;

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
                $"Embedded resource '{fullResourceName}' not found in assembly '{assembly.FullName}'."
            );
        }

        return stream;
    }

    private static JsonSerializerOptions CreateSerializerOptions(RegexJsonConverter regexConverter)
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { regexConverter },
        };
    }

    public static FrozenDictionary<string, string> LoadHints(string resourceName)
    {
        var regexConverter = new RegexJsonConverter();
        var serializerOptions = CreateSerializerOptions(regexConverter);
        using var stream = GetEmbeddedResourceStream(resourceName);
        using var reader = new StreamReader(stream);

        var hints =
            JsonSerializer.Deserialize<Dictionary<string, string>>(stream, serializerOptions) ?? [];

        return hints.ToFrozenDictionary();
    }
}
