using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UADetector.Tests.Helpers;

public static class FixtureLoader
{
    public static IEnumerable<T> Load<T>(string fixturePath)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .WithTypeConverter(new YamlEmptyStringToNullConverter())
            .Build();

        var fullPath = Path.Combine(AppContext.BaseDirectory, fixturePath);

        using var stream = new FileStream(fullPath, FileMode.Open);
        using var reader = new StreamReader(stream);
        return deserializer.Deserialize<IEnumerable<T>>(reader);
    }

    private class YamlEmptyStringToNullConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type) => type == typeof(string);

        public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            parser.TryConsume<Scalar>(out var scalar);

            return string.IsNullOrEmpty(scalar?.Value) ? null : scalar?.Value;
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            emitter.Emit(new Scalar(value?.ToString() ?? string.Empty));
        }
    }
}
