using System.Text;
using UaDetector.Abstractions;
using UaDetector.Abstractions.Models;
using UaDetector.Abstractions.Models.Browsers;
using UaDetector.SourceGenerator.Collections;
using UaDetector.SourceGenerator.Models;
using UaDetector.SourceGenerator.Utilities;

namespace UaDetector.SourceGenerator.Generators;

internal static class BrowserSourceGenerator
{
    private const string RegexMethodPrefix = "BrowserRegex";

    public static string Generate(
        string json,
        RegexSourceProperty regexSourceProperty,
        CombinedRegexProperty? combinedRegexProperty
    )
    {
        var list = JsonUtils.DeserializeJson<BrowserRule>(json);
        var regexDeclarations = GenerateRegexDeclarations(list);
        var collectionInitializer = GenerateCollectionInitializer(list, regexSourceProperty);

        var combinedRegexDeclaration = RegexBuilder.BuildCombinedRegexFieldDeclaration(
            combinedRegexProperty,
            string.Join("|", list.Reverse().Select(x => x.Regex))
        );

        return SourceCodeBuilder.BuildClassSourceCode(
            regexSourceProperty,
            regexDeclarations,
            collectionInitializer,
            combinedRegexDeclaration
        );
    }

    private static string GenerateRegexDeclarations(EquatableReadOnlyList<BrowserRule> list)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendLine(
                RegexBuilder.BuildRegexFieldDeclaration($"{RegexMethodPrefix}{i}", list[i].Regex)
            );
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string GenerateCollectionInitializer(
        EquatableReadOnlyList<BrowserRule> list,
        RegexSourceProperty regexSourceProperty
    )
    {
        if (list.Count == 0)
        {
            return "[]";
        }

        var sb = new IndentedStringBuilder();
        var engineType = $"global::{typeof(BrowserEngine).FullName}";

        sb.AppendLine("[").Indent();

        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendLine($"new {regexSourceProperty.ElementType}")
                .AppendLine("{")
                .Indent()
                .AppendLine($"{nameof(RuleDefinition<Browser>.Regex)} = {RegexMethodPrefix}{i},")
                .AppendLine(
                    $"{nameof(RuleDefinition<Browser>.Result)} = new {regexSourceProperty.ElementGenericType}"
                )
                .AppendLine("{")
                .Indent()
                .AppendLine($"{nameof(Browser.Name)} = \"{list[i].Name}\",");

            if (list[i].Version is not null)
            {
                sb.AppendLine($"{nameof(Browser.Version)} = \"{list[i].Version}\",");
            }

            if (list[i].Engine is not null)
            {
                sb.AppendLine($"{nameof(Browser.Engine)} = new {engineType}")
                    .AppendLine("{")
                    .Indent();

                if (list[i].Engine?.Default is { } defaultEngine)
                {
                    sb.AppendLine($"{nameof(Browser.Engine.Default)} = \"{defaultEngine}\",");
                }

                if (list[i].Engine?.Versions is { Count: > 0 } engineVersions)
                {
                    sb.AppendLine(
                            $"{nameof(Browser.Engine.Versions)} = new Dictionary<string, string>"
                        )
                        .AppendLine("{")
                        .Indent();

                    foreach (var version in engineVersions)
                    {
                        sb.AppendLine($"{{ \"{version.Key}\", \"{version.Value}\" }},");
                    }

                    sb.Unindent().AppendLine("},");
                }

                sb.Unindent().AppendLine("},");
            }

            sb.Unindent().AppendLine("},").Unindent().AppendLine("},");
        }

        sb.Unindent().AppendLine("]");

        return sb.ToString();
    }
}
