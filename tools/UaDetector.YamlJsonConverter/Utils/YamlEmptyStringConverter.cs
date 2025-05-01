using System.Text.RegularExpressions;

using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace UaDetector.YamlJsonConverter.Utils;

public sealed class YamlEmptyStringConverter : IYamlTypeConverter
{
    public bool Accepts(Type type) => type == typeof(string);

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        parser.TryConsume<Scalar>(out var scalar);

        if (scalar is null || string.IsNullOrEmpty(scalar.Value))
        {
            return null;
        }

        return scalar.Value;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        emitter.Emit(new Scalar(value?.ToString() ?? string.Empty));
    }
}

