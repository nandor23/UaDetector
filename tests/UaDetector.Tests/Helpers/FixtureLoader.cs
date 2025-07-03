using System.Text.Json;

namespace UaDetector.Tests.Helpers;

public static class FixtureLoader
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static async Task<List<T>> LoadAsync<T>(string fileName)
    {
        await using var stream = new FileStream(fileName, FileMode.Open);
        return await JsonSerializer.DeserializeAsync<List<T>>(stream, SerializerOptions) ?? [];
    }
}
