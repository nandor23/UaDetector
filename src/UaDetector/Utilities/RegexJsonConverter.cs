using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace UaDetector.Utilities;

internal sealed class RegexJsonConverter : JsonConverter<Regex>
{
    private readonly List<string> _patterns = [];
    private readonly string? _patternSuffix;

    public RegexJsonConverter(string? patternSuffix = null)
    {
        _patternSuffix = patternSuffix;
    }

    public override Regex Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        string? pattern = reader.GetString();

        if (pattern is null)
        {
            throw new JsonException("Regex pattern cannot be null");
        }

        var regex = _patternSuffix is null ? pattern : pattern + _patternSuffix;

        _patterns.Add(regex);

        return RegexUtilis.BuildUserAgentRegex(regex);
    }

    public override void Write(Utf8JsonWriter writer, Regex value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
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

        return RegexUtilis.BuildUserAgentRegex(sb.ToString());
    }
}
