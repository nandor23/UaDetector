using System.Diagnostics.CodeAnalysis;
using UaDetector.SourceGenerator.Collections;
using UaDetector.SourceGenerator.Models;
using UaDetector.SourceGenerator.Utilities;

namespace UaDetector.SourceGenerator.Generators;

public static class ClientSourceGenerator
{
    private const string ClientRegexPrefix = "ClientRegex";

    public static bool TryGenerate(
        string json,
        RegexSourceProperty regexSourceProperty,
        CombinedRegexProperty? combinedRegexProperty,
        [NotNullWhen(true)] out string? result
    )
    {
        if (!JsonUtils.TryDeserializeList<ClientRule>(json, out var list))
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

    private static string GenerateRegexDeclarations(EquatableReadOnlyList<ClientRule> list)
    {
        var sb = new IndentedStringBuilder();
        sb.Indent();

        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendLine(
                    RegexBuilder.BuildRegexFieldDeclaration(
                        $"{ClientRegexPrefix}{i}",
                        list[i].Regex
                    )
                )
                .AppendLine();
        }

        return sb.ToString()[..^1];
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

        sb.AppendLine("[").Indent().Indent();

        foreach (var client in list)
        {
            sb.AppendLine($"new {regexSourceProperty.ElementType}")
                .AppendLine("{")
                .Indent()
                .AppendLine($"Regex = {ClientRegexPrefix}{clientCount},")
                .AppendLine($"Name = \"{client.Name.EscapeStringLiteral()}\",");

            if (client.Version is not null)
            {
                sb.AppendLine($"Version = \"{client.Version.EscapeStringLiteral()}\",");
            }

            sb.Unindent().AppendLine("},");

            clientCount += 1;
        }

        sb.Unindent().AppendLine("];");

        return sb.ToString();
    }
}
