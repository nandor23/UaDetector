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
        var sb = new IndentedStringBuilder();
        sb.Indent();

        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendLine(
                    RegexBuilder.BuildRegexFieldDeclaration($"{BotRegexPrefix}{i}", list[i].Regex)
                )
                .AppendLine();
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
