using System.Text;
using System.Text.RegularExpressions;

using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace UADetector.Parsers;

internal sealed class YamlStringToRegexConverter : IYamlTypeConverter
{
    private readonly List<string> _patterns = [];
    private readonly string? _patternSuffix;


    public YamlStringToRegexConverter(string? patternSuffix = null)
    {
        _patternSuffix = patternSuffix;
    }

    public bool Accepts(Type type) => type == typeof(Regex);

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        parser.TryConsume<Scalar>(out var scalar);

        if (scalar is null)
        {
            return null;
        }

        var regex = string.IsNullOrEmpty(_patternSuffix) ? scalar.Value : scalar.Value + _patternSuffix;

        _patterns.Add(regex);

        return ParserExtensions.BuildUserAgentRegex(regex);
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        var regex = (Regex)value!;
        emitter.Emit(new Scalar(regex.ToString()));
    }

    public Regex BuildCombinedRegex()
    {
        var sb = new StringBuilder();

        if (_patterns.Count == 0)
        {
            return new Regex(string.Empty);
        }

        for (int i = _patterns.Count - 1; i > 0; i--)
        {
            sb.Append(_patterns[i]);
            sb.Append('|');
        }

        sb.Append(_patterns[0]);

        return ParserExtensions.BuildUserAgentRegex(sb.ToString());
    }
}
