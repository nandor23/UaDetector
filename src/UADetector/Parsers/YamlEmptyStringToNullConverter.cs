using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace UADetector.Parsers;

internal sealed class YamlEmptyStringToNullConverter : IYamlTypeConverter
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
