using System.Text;
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
        var list = JsonUtils.DeserializeList<BotRule>(json);
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

        var sb = new IndentedStringBuilder();
        int botCount = 0;

        sb.AppendLine("[").Indent();

        foreach (var bot in list)
        {
            sb.AppendLine($"new {regexSourceProperty.ElementType}")
                .AppendLine("{")
                .Indent()
                .AppendLine($"{nameof(BotRule.Regex)} = {BotRegexPrefix}{botCount},")
                .AppendLine($"{nameof(BotRule.Name)} = \"{bot.Name.EscapeStringLiteral()}\",");

            if (bot.Category is not null)
            {
                sb.AppendLine(
                    $"{nameof(BotRule.Category)} = (global::UaDetector.Abstractions.Models.Enums.BotCategory){bot.Category},"
                );
            }

            if (bot.Url is not null)
            {
                sb.AppendLine($"{nameof(BotRule.Url)} = \"{bot.Url.EscapeStringLiteral()}\",");
            }

            if (bot.Producer is not null)
            {
                sb.AppendLine(
                        $"{nameof(BotRule.Producer)} = new global::UaDetector.Abstractions.Models.Internal.BotProducer"
                    )
                    .AppendLine("{")
                    .Indent();

                if (bot.Producer.Name is not null)
                {
                    sb.AppendLine(
                        $"{nameof(BotProducerRule.Name)} = \"{bot.Producer.Name.EscapeStringLiteral()}\","
                    );
                }

                if (bot.Producer.Url is not null)
                {
                    sb.AppendLine(
                        $"{nameof(BotProducerRule.Url)} = \"{bot.Producer.Url.EscapeStringLiteral()}\","
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
