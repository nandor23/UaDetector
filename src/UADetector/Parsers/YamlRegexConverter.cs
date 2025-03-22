using System.Text;
using System.Text.RegularExpressions;

using UADetector.Models.Enums;

using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace UADetector.Parsers;

internal sealed class YamlRegexConverter : IYamlTypeConverter
{
    private readonly RegexPatternType _patternType;
    private readonly List<string> _patterns;

    public YamlRegexConverter(RegexPatternType patternType)
    {
        _patternType = patternType;
        _patterns = [];
    }

    private Regex BuildRegex(string pattern)
    {
        return _patternType switch
        {
            RegexPatternType.None => new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled),
            RegexPatternType.UserAgent => ParserExtensions.BuildUserAgentRegex(pattern),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public bool Accepts(Type type) => type == typeof(Regex);

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        parser.TryConsume<Scalar>(out var scalar);

        if (scalar is null)
        {
            return null;
        }

        _patterns.Add(scalar.Value);

        return BuildRegex(scalar.Value);
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        var regex = (Regex)value!;
        emitter.Emit(new Scalar(regex.ToString()));
    }

    public Regex GetOverallRegex()
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

        return BuildRegex(sb.ToString());
    }
}
