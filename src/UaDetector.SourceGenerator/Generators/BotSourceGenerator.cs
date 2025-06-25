using System.Text;
using UaDetector.Abstractions.Enums;
using UaDetector.Abstractions.Models;
using UaDetector.SourceGenerator.Collections;
using UaDetector.SourceGenerator.Models;
using UaDetector.SourceGenerator.Utilities;

namespace UaDetector.SourceGenerator.Generators;

internal sealed class BotSourceGenerator
{
    private const string BotRegexPrefix = "BotRegex";

    public static string Generate(
        string json,
        RegexSourceProperty regexSourceProperty,
        CombinedRegexProperty? combinedRegexProperty
    )
    {
        var list = JsonUtils.DeserializeJson<BotRule>(json);
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

    private static string GenerateRegexDeclarations(EquatableReadOnlyList<BotRule> list)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendLine(
                RegexBuilder.BuildRegexFieldDeclaration($"{BotRegexPrefix}{i}", list[i].Regex)
            );
            sb.AppendLine();
        }

        return sb.ToString();
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

        var categoryType = $"global::{typeof(BotCategory).FullName}";
        var producerType = $"global::{typeof(BotProducer).FullName}";
        var sb = new IndentedStringBuilder();
        int botCount = 0;

        sb.AppendLine("[").Indent();

        foreach (var bot in list)
        {
            sb.AppendLine($"new {regexSourceProperty.ElementType}")
                .AppendLine("{")
                .Indent()
                .AppendLine($"{nameof(Bot.Regex)} = {BotRegexPrefix}{botCount},")
                .AppendLine($"{nameof(Bot.Name)} = \"{bot.Name.EscapeStringLiteral()}\",");

            if (bot.Category is not null)
            {
                sb.AppendLine($"{nameof(Bot.Category)} = {categoryType}.{bot.Category},");
            }

            if (bot.Url is not null)
            {
                sb.AppendLine($"{nameof(Bot.Url)} = \"{bot.Url.EscapeStringLiteral()}\",");
            }

            if (bot.Producer is not null)
            {
                sb.AppendLine($"{nameof(Bot.Producer)} = new {producerType}")
                    .AppendLine("{")
                    .Indent();

                if (bot.Producer.Name is not null)
                {
                    sb.AppendLine(
                        $"{nameof(BotProducer.Name)} = \"{bot.Producer.Name.EscapeStringLiteral()}\","
                    );
                }

                if (bot.Producer.Url is not null)
                {
                    sb.AppendLine(
                        $"{nameof(BotProducer.Url)} = \"{bot.Producer.Url.EscapeStringLiteral()}\","
                    );
                }

                sb.Unindent().AppendLine("},");
            }

            sb.Unindent().AppendLine("},");

            botCount += 1;
        }

        sb.Unindent().AppendLine("]");

        return sb.ToString();
    }
}
