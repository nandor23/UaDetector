using System.Text;
using UaDetector.Models;
using UaDetector.Models.Browsers;
using UaDetector.SourceGenerator.Collections;
using UaDetector.SourceGenerator.Models;
using UaDetector.SourceGenerator.Utilities;

namespace UaDetector.SourceGenerator.Generators;

internal static class BrowserSourceGenerator
{
    private const string RegexMethodPrefix = "Regex";

    public static string Generate(PropertyDeclarationInfo property, string json)
    {
        var list = JsonUtils.DeserializeJson<BrowserRule>(json);
        var regexDeclarations = GenerateRegexDeclarations(list);
        var collectionInitializer = GenerateCollectionInitializer(list, property);

        return SourceCodeBuilder.BuildClassSourceCode(
            property,
            regexDeclarations,
            collectionInitializer
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
        PropertyDeclarationInfo property
    )
    {
        if (list.Count == 0)
        {
            return "[]";
        }

        var sb = new StringBuilder();
        var engineType = $"global::{typeof(Engine).FullName}";

        sb.AppendLine("[");

        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendIndentedLine(1, $"new {property.ElementType}")
                .AppendIndentedLine(1, "{")
                .AppendIndentedLine(
                    2,
                    $"{nameof(RuleDefinition<Browser>.Regex)} = {RegexMethodPrefix}{i},"
                )
                .AppendIndentedLine(
                    2,
                    $"{nameof(RuleDefinition<Browser>.Result)} = new {property.ElementGenericType}"
                )
                .AppendIndentedLine(2, "{")
                .AppendIndentedLine(3, $"{nameof(Browser.Name)} = \"{list[i].Name}\",");

            if (list[i].Version is not null)
            {
                sb.AppendIndentedLine(3, $"{nameof(Browser.Version)} = \"{list[i].Version}\",");
            }

            if (list[i].Engine is not null)
            {
                sb.AppendIndentedLine(3, $"{nameof(Browser.Engine)} = new {engineType}")
                    .AppendIndentedLine(3, "{");

                if (list[i].Engine?.Default is { } defaultEngine)
                {
                    sb.AppendIndentedLine(
                        4,
                        $"{nameof(Browser.Engine.Default)} = \"{defaultEngine}\","
                    );
                }

                if (list[i].Engine?.Versions is { Count: > 0 } engineVersions)
                {
                    sb.AppendIndentedLine(
                            4,
                            $"{nameof(Browser.Engine.Versions)} = new Dictionary<string, string>"
                        )
                        .AppendIndentedLine(4, "{");

                    foreach (var version in engineVersions)
                    {
                        sb.AppendIndentedLine(5, $"{{ \"{version.Key}\", \"{version.Value}\" }},");
                    }

                    sb.AppendIndentedLine(4, "},");
                }

                sb.AppendIndentedLine(3, "},");
            }

            sb.AppendIndentedLine(2, "},");
            sb.AppendIndentedLine(1, "},");
        }

        sb.AppendLine("]");

        return sb.ToString();
    }
}
