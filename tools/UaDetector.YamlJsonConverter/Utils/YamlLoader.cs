using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UaDetector.YamlJsonConverter.Utils;

public static class YamlLoader
{
    private static IDeserializer CreateDeserializer()
    {
        return new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .WithTypeConverter(new YamlRegexConverter())
            .WithTypeConverter(new YamlEmptyStringConverter())
            .Build();
    }

    public static List<T> LoadList<T>(string fileName)
    {
        var deserializer = CreateDeserializer();

        using var reader = new StreamReader(Path.Combine(AppContext.BaseDirectory, fileName));
        var entries = deserializer.Deserialize<List<T>>(reader);
        return entries;
    }

    public static OrderedDictionary<string, T> LoadDictionary<T>(string fileName)
    {
        var deserializer = CreateDeserializer();

        using var reader = new StreamReader(Path.Combine(AppContext.BaseDirectory, fileName));
        var entries = deserializer.Deserialize<OrderedDictionary<string, T>>(reader);
        return entries;
    }
}
