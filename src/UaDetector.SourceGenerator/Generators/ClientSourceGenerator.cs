using System.Text;
using UaDetector.Abstractions.Models.Internal;
using UaDetector.SourceGenerator.Collections;
using UaDetector.SourceGenerator.Models;
using UaDetector.SourceGenerator.Utilities;

namespace UaDetector.SourceGenerator.Generators;

internal static class ClientSourceGenerator
{
    private const string ClientRegexPrefix = "ClientRegex";

    public static string Generate(
        string json,
        RegexSourceProperty regexSourceProperty,
        CombinedRegexProperty? combinedRegexProperty
    )
    {
        var list = JsonUtils.DeserializeList<ClientRule>(json);
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

    private static string GenerateRegexDeclarations(EquatableReadOnlyList<ClientRule> list)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendLine(
                RegexBuilder.BuildRegexFieldDeclaration($"{ClientRegexPrefix}{i}", list[i].Regex)
            );
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string GenerateCollectionInitializer(
        EquatableReadOnlyList<ClientRule> list,
        RegexSourceProperty regexSourceProperty
    )
    {
        if (list.Count == 0)
        {
            return "[]";
        }

        var sb = new IndentedStringBuilder();
        int clientCount = 0;

        sb.AppendLine("[").Indent();

        foreach (var client in list)
        {
            sb.AppendLine($"new {regexSourceProperty.ElementType}")
                .AppendLine("{")
                .Indent()
                .AppendLine($"{nameof(Client.Regex)} = {ClientRegexPrefix}{clientCount},")
                .AppendLine($"{nameof(Client.Name)} = \"{client.Name.EscapeStringLiteral()}\",");

            if (client.Version is not null)
            {
                sb.AppendLine(
                    $"{nameof(Client.Version)} = \"{client.Version.EscapeStringLiteral()}\","
                );
            }

            sb.Unindent().AppendLine("},");

            clientCount += 1;
        }

        sb.Unindent().AppendLine("]");

        return sb.ToString();
    }
}
