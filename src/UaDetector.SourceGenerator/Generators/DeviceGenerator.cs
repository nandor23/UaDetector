using System.Diagnostics.CodeAnalysis;
using UaDetector.SourceGenerator.Collections;
using UaDetector.SourceGenerator.Models;
using UaDetector.SourceGenerator.Utilities;

namespace UaDetector.SourceGenerator.Generators;

public static class DeviceGenerator
{
    private const string DeviceRegexPrefix = "DeviceRegex";
    private const string ModelRegexPrefix = "ModelRegex";

    public static bool TryGenerate(
        bool isLiteMode,
        string json,
        RegexSourceProperty regexSourceProperty,
        CombinedRegexProperty? combinedRegexProperty,
        [NotNullWhen(true)] out string? result
    )
    {
        if (!JsonUtils.TryDeserializeList<DeviceRule>(json, out var list))
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
        EquatableReadOnlyList<DeviceRule> list,
        bool isLiteMode
    )
    {
        var sb = new IndentedStringBuilder();
        sb.Indent();

        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendLine(
                    RegexBuilder.BuildRegexFieldDeclaration(
                        $"{DeviceRegexPrefix}{i}",
                        list[i].Regex,
                        isLiteMode
                    )
                )
                .AppendLine();
        }

        int modelCount = 0;

        foreach (var device in list)
        {
            if (device.ModelVariants is not null)
            {
                foreach (var model in device.ModelVariants)
                {
                    sb.AppendLine(
                            RegexBuilder.BuildRegexFieldDeclaration(
                                $"{ModelRegexPrefix}{modelCount}",
                                model.Regex,
                                isLiteMode
                            )
                        )
                        .AppendLine();

                    modelCount += 1;
                }
            }
        }

        return sb.ToString()[..^1];
    }

    private static string GenerateCollectionInitializer(
        EquatableReadOnlyList<DeviceRule> list,
        RegexSourceProperty regexSourceProperty
    )
    {
        if (list.Count == 0)
        {
            return "[]";
        }

        var sb = new IndentedStringBuilder();
        int deviceCount = 0;
        int modelCount = 0;

        sb.AppendLine("[").Indent().Indent();

        foreach (var device in list)
        {
            sb.AppendLine($"new {regexSourceProperty.ElementType}")
                .AppendLine("{")
                .Indent()
                .AppendLine($"Regex = {DeviceRegexPrefix}{deviceCount},")
                .AppendLine($"Brand = \"{device.Brand.EscapeStringLiteral()}\",");

            if (device.Type is not null)
            {
                sb.AppendLine(
                    $"Type = (global::UaDetector.Abstractions.Enums.DeviceType){device.Type},"
                );
            }

            if (device.Model is not null)
            {
                sb.AppendLine($"Model = \"{device.Model.EscapeStringLiteral()}\",");
            }

            if (device.ModelVariants is not null)
            {
                sb.AppendLine("ModelVariants = new global::UaDetector.Models.DeviceModel[]")
                    .AppendLine("{")
                    .Indent();

                foreach (var model in device.ModelVariants)
                {
                    sb.AppendLine("new global::UaDetector.Models.DeviceModel")
                        .AppendLine("{")
                        .Indent()
                        .AppendLine($"Regex = {ModelRegexPrefix}{modelCount},");

                    if (model.Type is not null)
                    {
                        sb.AppendLine(
                            $"Type = (global::UaDetector.Abstractions.Enums.DeviceType){model.Type},"
                        );
                    }

                    if (model.Brand is not null)
                    {
                        sb.AppendLine($"Brand = \"{model.Brand.EscapeStringLiteral()}\",");
                    }

                    if (model.Name is not null)
                    {
                        sb.AppendLine($"Name = \"{model.Name.EscapeStringLiteral()}\",");
                    }

                    sb.Unindent().AppendLine("},");

                    modelCount += 1;
                }

                sb.Unindent().AppendLine("},");
            }

            sb.Unindent().AppendLine("},");

            deviceCount += 1;
        }

        sb.Unindent().AppendLine("];");

        return sb.ToString();
    }
}
