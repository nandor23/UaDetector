using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Abstractions.Enums;
using UaDetector.Abstractions.Models;
using UaDetector.Catalogs;
using UaDetector.Models;

namespace UaDetector.Parsers.Devices;

internal abstract class DeviceParserBase
{
    internal static readonly FrozenDictionary<string, BrandCode> BrandNameMappings;

    static DeviceParserBase()
    {
        BrandNameMappings = BrandCatalog
            .BrandCodeMappings.ToDictionary(e => e.Value, e => e.Key)
            .ToFrozenDictionary();
    }

    private static string? BuildModel(string model, Match match)
    {
        model = ParserExtensions.FormatWithMatch(model, match).Replace('_', ' ');
        model = Regex.Replace(model, " TD$", string.Empty, RegexOptions.IgnoreCase);

        return model is null or { Length: 0 } or "Build" ? null : model.Trim();
    }

    public abstract bool TryParse(
        string userAgent,
        [NotNullWhen(true)] out DeviceInfoInternal? result
    );

    protected static bool TryParse(
        string userAgent,
        IEnumerable<Device> devices,
        [NotNullWhen(true)] out DeviceInfoInternal? result
    )
    {
        string? brand = null;
        Match? match = null;
        Device? matchedDevice = null;

        foreach (var device in devices)
        {
            match = device.Regex.Match(userAgent);

            if (match.Success)
            {
                brand = device.Brand;
                matchedDevice = device;
                break;
            }
        }

        if (match is null || !match.Success)
        {
            result = null;
            return false;
        }

        DeviceType? type = matchedDevice?.Type;
        string? model = null;

        if (matchedDevice?.Model?.Length > 0)
        {
            model = BuildModel(matchedDevice.Model, match);
        }

        if (matchedDevice?.ModelVariants is not null)
        {
            Match? modelMatch = null;
            DeviceModel? deviceModel = null;

            foreach (var modelVariant in matchedDevice.ModelVariants)
            {
                modelMatch = modelVariant.Regex.Match(userAgent);

                if (modelMatch.Success)
                {
                    deviceModel = modelVariant;
                    break;
                }
            }

            if (modelMatch is null || !modelMatch.Success)
            {
                result = new DeviceInfoInternal
                {
                    Type = type,
                    Brand = brand,
                    Model = model,
                };

                return true;
            }

            if (deviceModel?.Name?.Length > 0)
            {
                model = BuildModel(deviceModel.Name, modelMatch);
            }

            if (deviceModel?.Brand?.Length > 0 && BrandNameMappings.ContainsKey(deviceModel.Brand))
            {
                brand = deviceModel.Brand;
            }

            if (deviceModel?.Type is not null)
            {
                type = deviceModel.Type;
            }
        }

        result = new DeviceInfoInternal
        {
            Type = type,
            Brand = brand,
            Model = model,
        };
        return true;
    }
}
