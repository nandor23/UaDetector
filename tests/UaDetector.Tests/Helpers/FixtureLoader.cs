using System.Text.Json;

namespace UaDetector.Tests.Helpers;

public static class FixtureLoader
{
    private static readonly JsonSerializerOptions SerializerOptions =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            RespectRequiredConstructorParameters = true,
        };

    public async static Task<IEnumerable<T>> LoadAsync<T>(string fileName)
    {
        await using var stream = new FileStream(fileName, FileMode.Open);
        return await JsonSerializer.DeserializeAsync<IEnumerable<T>>(stream, SerializerOptions) ?? [];
    }
}
