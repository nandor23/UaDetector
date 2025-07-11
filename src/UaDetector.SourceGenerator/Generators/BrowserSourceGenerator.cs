using System.Diagnostics.CodeAnalysis;
using UaDetector.SourceGenerator.Collections;
using UaDetector.SourceGenerator.Models;
using UaDetector.SourceGenerator.Utilities;

namespace UaDetector.SourceGenerator.Generators;

public static class BrowserSourceGenerator
{
    private const string BrowserRegexPrefix = "BrowserRegex";

    public static bool TryGenerate(
        string json,
        RegexSourceProperty regexSourceProperty,
        CombinedRegexProperty? combinedRegexProperty,
        [NotNullWhen(true)] out string? result
    )
    {
        if (!JsonUtils.TryDeserializeList<BrowserRule>(json, out var list))
        {
            result = null;
            return false;
        }

        var regexDeclarations = GenerateRegexDeclarations(list.Value);
        var collectionInitializer = GenerateCollectionInitializer(list.Value, regexSourceProperty);

        var combinedRegexDeclaration = RegexBuilder.BuildCombinedRegexFieldDeclaration(
            combinedRegexProperty,
            string.Join("|", list.Value.Reverse().Select(x => x.Regex))
        );

        result = SourceCodeBuilder.BuildClassSourceCode(
            regexSourceProperty,
            regexDeclarations,
            collectionInitializer,
            combinedRegexDeclaration
        );

        return true;
    }

    private static string GenerateRegexDeclarations(EquatableReadOnlyList<BrowserRule> list)
    {
        var sb = new IndentedStringBuilder();
        sb.Indent();

        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendLine(
                    RegexBuilder.BuildRegexFieldDeclaration(
                        $"{BrowserRegexPrefix}{i}",
                        list[i].Regex
                    )
                )
                .AppendLine();
        }

        return sb.ToString()[..^1];
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

        sb.AppendLine("[").Indent().Indent();

        foreach (var browser in list)
        {
            sb.AppendLine($"new {regexSourceProperty.ElementType}")
                .AppendLine("{")
                .Indent()
                .AppendLine($"Regex = {BrowserRegexPrefix}{browserCount},")
                .AppendLine($"Name = \"{browser.Name.EscapeStringLiteral()}\",");

            if (browser.Version is not null)
            {
                sb.AppendLine($"Version = \"{browser.Version.EscapeStringLiteral()}\",");
            }

            if (browser.Engine is not null)
            {
                sb.AppendLine("Engine = new global::UaDetector.Models.BrowserEngine")
                    .AppendLine("{")
                    .Indent();

                if (browser.Engine?.Default is not null)
                {
                    sb.AppendLine($"Default = \"{browser.Engine.Default.EscapeStringLiteral()}\",");
                }

                if (browser.Engine?.Versions is not null)
                {
                    sb.AppendLine(
                            "Versions = new global::System.Collections.Generic.Dictionary<string, string>"
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

        sb.Unindent().AppendLine("];");

        return sb.ToString();
    }
}
