using UaDetector.SourceGenerator.Collections;
using UaDetector.SourceGenerator.Models;
using UaDetector.SourceGenerator.Utilities;

namespace UaDetector.SourceGenerator.Generators;

internal static class VendorFragmentSourceGenerator
{
    private const string FragmentRegexPrefix = "VendorFragmentRegex";

    public static string Generate(
        string json,
        RegexSourceProperty regexSourceProperty,
        CombinedRegexProperty? combinedRegexProperty
    )
    {
        var list = JsonUtils.DeserializeList<VendorFragmentRule>(json);
        var regexDeclarations = GenerateRegexDeclarations(list);
        var collectionInitializer = GenerateCollectionInitializer(list, regexSourceProperty);

        var combinedRegexDeclaration = RegexBuilder.BuildCombinedRegexFieldDeclaration(
            combinedRegexProperty,
            string.Join(
                "|",
                list.Reverse()
                    .SelectMany(x =>
                        x.Regexes.Select(regex => $"{regex}{regexSourceProperty.RegexSuffix}")
                    )
            )
        );

        return SourceCodeBuilder.BuildClassSourceCode(
            regexSourceProperty,
            regexDeclarations,
            collectionInitializer,
            combinedRegexDeclaration
        );
    }

    private static string GenerateRegexDeclarations(EquatableReadOnlyList<VendorFragmentRule> list)
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
                            regex
                        )
                    )
                    .AppendLine();

                fragmentCount += 1;
            }
        }

        return sb.ToString();
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

        sb.AppendLine("[").Indent();

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
