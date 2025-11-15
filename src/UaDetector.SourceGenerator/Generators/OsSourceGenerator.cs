using System.Diagnostics.CodeAnalysis;
using UaDetector.SourceGenerator.Collections;
using UaDetector.SourceGenerator.Models;
using UaDetector.SourceGenerator.Utilities;

namespace UaDetector.SourceGenerator.Generators;

public static class OsSourceGenerator
{
    private const string OsRegexPrefix = "OsRegex";
    private const string VersionRegexPrefix = "VersionRegex";

    public static bool TryGenerate(
        bool isLiteMode,
        string json,
        RegexSourceProperty regexSourceProperty,
        CombinedRegexProperty? combinedRegexProperty,
        [NotNullWhen(true)] out string? result
    )
    {
        if (!JsonUtils.TryDeserializeList<OsRule>(json, out var list))
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
        EquatableReadOnlyList<OsRule> list,
        bool isLiteMode
    )
    {
        var sb = new IndentedStringBuilder();
        sb.Indent();

        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendLine(
                    RegexBuilder.BuildRegexFieldDeclaration(
                        $"{OsRegexPrefix}{i}",
                        list[i].Regex,
                        isLiteMode
                    )
                )
                .AppendLine();
        }

        int versionCount = 0;

        foreach (var os in list)
        {
            if (os.Versions is not null)
            {
                foreach (var version in os.Versions)
                {
                    sb.AppendLine(
                            RegexBuilder.BuildRegexFieldDeclaration(
                                $"{VersionRegexPrefix}{versionCount}",
                                version.Regex,
                                isLiteMode
                            )
                        )
                        .AppendLine();

                    versionCount += 1;
                }
            }
        }

        return sb.ToString()[..^1];
    }

    private static string GenerateCollectionInitializer(
        EquatableReadOnlyList<OsRule> list,
        RegexSourceProperty regexSourceProperty
    )
    {
        if (list.Count == 0)
        {
            return "[]";
        }

        var sb = new IndentedStringBuilder();
        int osCount = 0;
        int versionCount = 0;

        sb.AppendLine("[").Indent().Indent();

        foreach (var os in list)
        {
            sb.AppendLine($"new {regexSourceProperty.ElementType}")
                .AppendLine("{")
                .Indent()
                .AppendLine($"Regex = {OsRegexPrefix}{osCount},")
                .AppendLine($"Name = \"{os.Name.EscapeStringLiteral()}\",");

            if (os.Version is not null)
            {
                sb.AppendLine($"Version = \"{os.Version.EscapeStringLiteral()}\",");
            }

            if (os.Versions is not null)
            {
                sb.AppendLine("Versions = new global::UaDetector.Models.OsVersion[]")
                    .AppendLine("{")
                    .Indent();

                foreach (var osVersion in os.Versions)
                {
                    sb.AppendLine("new global::UaDetector.Models.OsVersion")
                        .AppendLine("{")
                        .Indent()
                        .AppendLine($"Regex = {VersionRegexPrefix}{versionCount},")
                        .AppendLine($"Version = \"{osVersion.Version.EscapeStringLiteral()}\",")
                        .Unindent()
                        .AppendLine("},");

                    versionCount += 1;
                }

                sb.Unindent().AppendLine("},");
            }

            sb.Unindent().AppendLine("},");

            osCount += 1;
        }

        sb.Unindent().AppendLine("];");

        return sb.ToString();
    }
}
