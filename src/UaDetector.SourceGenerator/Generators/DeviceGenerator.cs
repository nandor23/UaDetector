using System.Text;
using UaDetector.Abstractions.Enums;
using UaDetector.Abstractions.Models;
using UaDetector.SourceGenerator.Collections;
using UaDetector.SourceGenerator.Models;
using UaDetector.SourceGenerator.Utilities;

namespace UaDetector.SourceGenerator.Generators;

internal sealed class DeviceGenerator
{
    private const string DeviceRegexPrefix = "DeviceRegex";
    private const string ModelRegexPrefix = "ModelRegex";

    public static string Generate(
        string json,
        RegexSourceProperty regexSourceProperty,
        CombinedRegexProperty? combinedRegexProperty
    )
    {
        var list = JsonUtils.DeserializeJson<DeviceRule>(json);
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

    private static string GenerateRegexDeclarations(EquatableReadOnlyList<DeviceRule> list)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendLine(
                RegexBuilder.BuildRegexFieldDeclaration($"{DeviceRegexPrefix}{i}", list[i].Regex)
            );
            sb.AppendLine();
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
                            model.Regex
                        )
                    );
                    sb.AppendLine();

                    modelCount += 1;
                }
            }
        }

        return sb.ToString();
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

        var deviceModelType = $"global::{typeof(DeviceModel).FullName}";
        var enumType = $"global::{typeof(DeviceType).FullName}";
        var sb = new IndentedStringBuilder();
        int deviceCount = 0;
        int modelCount = 0;

        sb.AppendLine("[").Indent();
        
        foreach (var device in list)
        {
            sb.AppendLine($"new {regexSourceProperty.ElementType}")
                .AppendLine("{")
                .Indent()
                .AppendLine($"{nameof(Device.Regex)} = {DeviceRegexPrefix}{deviceCount},")
                .AppendLine($"{nameof(Device.Brand)} = \"{device.Brand.EscapeStringLiteral()}\",");

            if (device.Type is not null)
            {
                sb.AppendLine($"{nameof(Device.Type)} = {enumType}.{device.Type},");
            }

            if (device.Model is not null)
            {
                sb.AppendLine($"{nameof(Device.Model)} = \"{device.Model.EscapeStringLiteral()}\",");
            }

            if (device.ModelVariants is not null)
            {
                sb.AppendLine($"{nameof(Device.ModelVariants)} = new {deviceModelType}[]")
                    .AppendLine("{")
                    .Indent();

                foreach (var model in device.ModelVariants)
                {
                    sb.AppendLine($"new {deviceModelType}")
                        .AppendLine("{")
                        .Indent()
                        .AppendLine(
                            $"{nameof(DeviceModel.Regex)} = {ModelRegexPrefix}{modelCount},"
                        );

                    if (model.Type is not null)
                    {
                        sb.AppendLine($"{nameof(DeviceModel.Type)} = {enumType}.{model.Type},");
                    }

                    if (model.Brand is not null)
                    {
                        sb.AppendLine($"{nameof(DeviceModel.Brand)} = \"{model.Brand.EscapeStringLiteral()}\",");
                    }

                    if (model.Name is not null)
                    {
                        sb.AppendLine(
                            $"{nameof(DeviceModel.Name)} = \"{model.Name.EscapeStringLiteral()}\","
                        );
                    }

                    sb.Unindent().AppendLine("},");

                    modelCount += 1;
                }

                sb.Unindent().AppendLine("},");
            }

            sb.Unindent().AppendLine("},");

            deviceCount += 1;
        }

        sb.Unindent().AppendLine("]");

        return sb.ToString();
    }
}
