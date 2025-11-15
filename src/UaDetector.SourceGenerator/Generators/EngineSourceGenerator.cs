using System.Diagnostics.CodeAnalysis;
using UaDetector.SourceGenerator.Collections;
using UaDetector.SourceGenerator.Models;
using UaDetector.SourceGenerator.Utilities;

namespace UaDetector.SourceGenerator.Generators;

public static class EngineSourceGenerator
{
    private const string EngineRegexPrefix = "EngineRegex";

    public static bool TryGenerate(
        bool isLiteMode,
        string json,
        RegexSourceProperty regexSourceProperty,
        CombinedRegexProperty? combinedRegexProperty,
        [NotNullWhen(true)] out string? result
    )
    {
        if (!JsonUtils.TryDeserializeList<EngineRule>(json, out var list))
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
        EquatableReadOnlyList<EngineRule> list,
        bool isLiteMode
    )
    {
        var sb = new IndentedStringBuilder();
        sb.Indent();

        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendLine(
                    RegexBuilder.BuildRegexFieldDeclaration(
                        $"{EngineRegexPrefix}{i}",
                        list[i].Regex,
                        isLiteMode
                    )
                )
                .AppendLine();
        }

        return sb.ToString()[..^1];
    }

    private static string GenerateCollectionInitializer(
        EquatableReadOnlyList<EngineRule> list,
        RegexSourceProperty regexSourceProperty
    )
    {
        if (list.Count == 0)
        {
            return "[]";
        }

        var sb = new IndentedStringBuilder();

        sb.AppendLine("[").Indent().Indent();

        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendLine($"new {regexSourceProperty.ElementType}")
                .AppendLine("{")
                .Indent()
                .AppendLine($"Regex = {EngineRegexPrefix}{i},")
                .AppendLine($"Name = \"{list[i].Name.EscapeStringLiteral()}\",")
                .Unindent()
                .AppendLine("},");
        }

        sb.Unindent().AppendLine("];");

        return sb.ToString();
    }
}
