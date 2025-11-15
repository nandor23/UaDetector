using System.Diagnostics.CodeAnalysis;
using UaDetector.SourceGenerator.Collections;
using UaDetector.SourceGenerator.Models;
using UaDetector.SourceGenerator.Utilities;

namespace UaDetector.SourceGenerator.Generators;

public static class VendorFragmentSourceGenerator
{
    private const string FragmentRegexPrefix = "VendorFragmentRegex";

    public static bool TryGenerate(
        bool isLiteMode,
        string json,
        RegexSourceProperty regexSourceProperty,
        CombinedRegexProperty? combinedRegexProperty,
        [NotNullWhen(true)] out string? result
    )
    {
        if (!JsonUtils.TryDeserializeList<VendorFragmentRule>(json, out var list))
        {
            result = null;
            return false;
        }
        var regexDeclarations = GenerateRegexDeclarations(list.Value, isLiteMode);
        var collectionInitializer = GenerateCollectionInitializer(list.Value, regexSourceProperty);

        var combinedRegexDeclaration = RegexBuilder.BuildCombinedRegexFieldDeclaration(
            combinedRegexProperty,
            string.Join(
                "|",
                list.Value.Reverse()
                    .SelectMany(x =>
                        x.Regexes.Select(regex => $"{regex}{regexSourceProperty.RegexSuffix}")
                    )
            ),
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
        EquatableReadOnlyList<VendorFragmentRule> list,
        bool isLiteMode
    )
    {
        int fragmentCount = 0;
        var sb = new IndentedStringBuilder();
        sb.Indent();

        foreach (var fragment in list)
        {
            foreach (var regex in fragment.Regexes)
            {
                sb.AppendLine(
                        RegexBuilder.BuildRegexFieldDeclaration(
                            $"{FragmentRegexPrefix}{fragmentCount}",
                            regex,
                            isLiteMode
                        )
                    )
                    .AppendLine();

                fragmentCount += 1;
            }
        }

        return sb.ToString()[..^1];
    }

    private static string GenerateCollectionInitializer(
        EquatableReadOnlyList<VendorFragmentRule> list,
        RegexSourceProperty regexSourceProperty
    )
    {
        if (list.Count == 0)
        {
            return "[]";
        }

        var sb = new IndentedStringBuilder();
        int fragmentCount = 0;

        sb.AppendLine("[").Indent().Indent();

        foreach (var fragment in list)
        {
            sb.AppendLine($"new {regexSourceProperty.ElementType}")
                .AppendLine("{")
                .Indent()
                .AppendLine($"Brand = \"{fragment.Brand.EscapeStringLiteral()}\",")
                .AppendLine("Regexes = new global::System.Text.RegularExpressions.Regex[]")
                .AppendLine("{")
                .Indent();

            for (int i = 0; i < fragment.Regexes.Count; i++)
            {
                sb.AppendLine($"{FragmentRegexPrefix}{fragmentCount},");
                fragmentCount += 1;
            }

            sb.Unindent().AppendLine("},").Unindent().AppendLine("},");
        }

        sb.Unindent().AppendLine("];");

        return sb.ToString();
    }
}
