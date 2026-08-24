using System.Text.Json;
using System.Text.Json.Serialization;
using Shouldly;

namespace UaDetector.Tests.Helpers;

public static class FixtureAssert
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
    };

    public static void ForEach<T>(string fixturePath, IEnumerable<T> fixtures, Action<T> assert)
    {
        int index = 0;

        foreach (var fixture in fixtures)
        {
            try
            {
                assert(fixture);
            }
            catch (Exception exception)
            {
                throw new ShouldAssertException(
                    $"""
                    {exception.Message}

                    Fixture file: {fixturePath}
                    Fixture index: {index}
                    Fixture: {Serialize(fixture)}
                    """,
                    exception
                );
            }

            index++;
        }
    }

    private static string Serialize<T>(T fixture)
    {
        try
        {
            return JsonSerializer.Serialize(fixture, SerializerOptions);
        }
        catch (Exception exception)
        {
            return $"<unable to serialize fixture: {exception.Message}>";
        }
    }
}
