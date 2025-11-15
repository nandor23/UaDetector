using System.Diagnostics.CodeAnalysis;
using UaDetector.SourceGenerator.Collections;
using UaDetector.SourceGenerator.Models;
using UaDetector.SourceGenerator.Utilities;

namespace UaDetector.SourceGenerator.Generators;

public static class BotSourceGenerator
{
    private const string BotRegexPrefix = "BotRegex";

    public static bool TryGenerate(
        bool isLiteMode,
        string json,
        RegexSourceProperty regexSourceProperty,
        CombinedRegexProperty? combinedRegexProperty,
        [NotNullWhen(true)] out string? result
    )
    {
        if (!JsonUtils.TryDeserializeList<BotRule>(json, out var list))
        {
            result = null;
            return false;
        }

        var regexDeclarations = GenerateRegexDeclarations(list.Value, isLiteMode);
        var collectionInitializer = GenerateCollectionInitializer(list.Value, regexSourceProperty);

        var combinedRegexDeclaration = RegexBuilder.BuildCombinedRegexFieldDeclaration(
            combinedRegexProperty,
            string.Join("|", list.Value.Reverse().Select(x => x.Regex)),
            isLiteMode
        );

        result = SourceCodeBuilder.BuildClassSourceCode(
            regexSourceProperty,
            regexDeclarations,
            collectionInitializer,
            combinedRegexDeclaration
        );

        return true;
    }

    private static string GenerateRegexDeclarations(
        EquatableReadOnlyList<BotRule> list,
        bool isLiteMode
    )
    {
        var sb = new IndentedStringBuilder();
        sb.Indent();

        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendLine(
                    RegexBuilder.BuildRegexFieldDeclaration(
                        $"{BotRegexPrefix}{i}",
                        list[i].Regex,
                        isLiteMode
                    )
                )
                .AppendLine();
        }

        return sb.ToString()[..^1];
    }

    private static string GenerateCollectionInitializer(
        EquatableReadOnlyList<BotRule> list,
        RegexSourceProperty regexSourceProperty
    )
    {
        if (list.Count == 0)
        {
            return "[]";
        }

        var sb = new IndentedStringBuilder();
        int botCount = 0;

        sb.AppendLine("[").Indent().Indent();

        foreach (var bot in list)
        {
            sb.AppendLine($"new {regexSourceProperty.ElementType}")
                .AppendLine("{")
                .Indent()
                .AppendLine($"Regex = {BotRegexPrefix}{botCount},")
                .AppendLine($"Name = \"{bot.Name.EscapeStringLiteral()}\",");

            if (bot.Category is not null)
            {
                sb.AppendLine(
                    $"Category = (global::UaDetector.Abstractions.Enums.BotCategory){bot.Category},"
                );
            }

            if (bot.Url is not null)
            {
                sb.AppendLine($"Url = \"{bot.Url.EscapeStringLiteral()}\",");
            }

            if (bot.Producer is not null)
            {
                sb.AppendLine("Producer = new global::UaDetector.Models.BotProducer")
                    .AppendLine("{")
                    .Indent();

                if (bot.Producer.Name is not null)
                {
                    sb.AppendLine($"Name = \"{bot.Producer.Name.EscapeStringLiteral()}\",");
                }

                if (bot.Producer.Url is not null)
                {
                    sb.AppendLine($"Url = \"{bot.Producer.Url.EscapeStringLiteral()}\",");
                }

                sb.Unindent().AppendLine("},");
            }

            sb.Unindent().AppendLine("},");

            botCount += 1;
        }

        sb.Unindent().AppendLine("];");

        return sb.ToString();
    }
}
