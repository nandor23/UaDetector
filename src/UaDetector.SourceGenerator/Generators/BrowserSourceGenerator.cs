using System.Text;
using UaDetector.Abstractions.Models.Internal;
using UaDetector.SourceGenerator.Collections;
using UaDetector.SourceGenerator.Models;
using UaDetector.SourceGenerator.Utilities;

namespace UaDetector.SourceGenerator.Generators;

internal static class BrowserSourceGenerator
{
    private const string BrowserRegexPrefix = "BrowserRegex";

    public static string Generate(
        string json,
        RegexSourceProperty regexSourceProperty,
        CombinedRegexProperty? combinedRegexProperty
    )
    {
        var list = JsonUtils.DeserializeList<BrowserRule>(json);
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
                RegexBuilder.BuildRegexFieldDeclaration($"{BrowserRegexPrefix}{i}", list[i].Regex)
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
        int browserCount = 0;

        sb.AppendLine("[").Indent();

        foreach (var browser in list)
        {
            sb.AppendLine($"new {regexSourceProperty.ElementType}")
                .AppendLine("{")
                .Indent()
                .AppendLine($"{nameof(Browser.Regex)} = {BrowserRegexPrefix}{browserCount},")
                .AppendLine($"{nameof(Browser.Name)} = \"{browser.Name.EscapeStringLiteral()}\",");

            if (browser.Version is not null)
            {
                sb.AppendLine(
                    $"{nameof(Browser.Version)} = \"{browser.Version.EscapeStringLiteral()}\","
                );
            }

            if (browser.Engine is not null)
            {
                sb.AppendLine(
                        $"{nameof(Browser.Engine)} = new global::UaDetector.Abstractions.Models.Internal.BrowserEngine"
                    )
                    .AppendLine("{")
                    .Indent();

                if (browser.Engine?.Default is not null)
                {
                    sb.AppendLine(
                        $"{nameof(Browser.Engine.Default)} = \"{browser.Engine.Default.EscapeStringLiteral()}\","
                    );
                }

                if (browser.Engine?.Versions is not null)
                {
                    sb.AppendLine(
                            $"{nameof(Browser.Engine.Versions)} = new global::System.Collections.Generic.Dictionary<string, string>"
                        )
                        .AppendLine("{")
                        .Indent();

                    foreach (var version in browser.Engine.Versions)
                    {
                        sb.AppendLine(
                            $"{{ \"{version.Key}\", \"{version.Value.EscapeStringLiteral()}\" }},"
                        );
                    }

                    sb.Unindent().AppendLine("},");
                }

                sb.Unindent().AppendLine("},");
            }

            sb.Unindent().AppendLine("},");

            browserCount += 1;
        }

        sb.Unindent().AppendLine("]");

        return sb.ToString();
    }
}
