using System.Text.RegularExpressions;

using UADetector.Models.Enums;

using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace UADetector.Parsers;

internal sealed class YamlRegexConverter : IYamlTypeConverter
{
    private readonly RegexPatternType _patternType;


    public YamlRegexConverter(RegexPatternType patternType)
    {
        _patternType = patternType;
    }

    public bool Accepts(Type type) => type == typeof(Regex);

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        parser.TryConsume<Scalar>(out var scalar);

        if (scalar is null)
        {
            return null;
        }

        return _patternType switch
        {
            RegexPatternType.None => new Regex(scalar.Value, RegexOptions.Compiled | RegexOptions.IgnoreCase),
            RegexPatternType.UserAgent => ParserExtensions.BuildUserAgentRegex(scalar.Value),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        var regex = (Regex)value!;
        emitter.Emit(new Scalar(regex.ToString()));
    }
}
