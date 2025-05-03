using System.Collections.Frozen;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using UaDetector.Models.Enums;
using UaDetector.Parsers;
using UaDetector.Parsers.Devices;
using UaDetector.Regexes.Models;
using UaDetector.Results;
using UaDetector.Tests.Fixtures.Models;
using UaDetector.YamlJsonConverter.Fixtures;
using UaDetector.YamlJsonConverter.Models;
using UaDetector.YamlJsonConverter.Utils;

namespace UaDetector.YamlJsonConverter;

public static class YamlToJsonConverter
{
    private const string BaseDirectory = "Inputs";
    private const string DevicesFile = "devices";
    private const string CollectionFile = "collection";

    private static readonly FrozenDictionary<string, ClientType> ClientTypeMapping = new Dictionary<
        string,
        ClientType
    >
    {
        { "mobile app", ClientType.MobileApp },
        { "mediaplayer", ClientType.MediaPlayer },
        { "library", ClientType.Library },
        { "feed reader", ClientType.FeedReader },
        { "pim", ClientType.PersonalInformationManager },
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, DeviceType> DeviceTypeMapping = new Dictionary<
        string,
        DeviceType
    >
    {
        { "desktop", DeviceType.Desktop },
        { "smartphone", DeviceType.Smartphone },
        { "tablet", DeviceType.Tablet },
        { "feature phone", DeviceType.FeaturePhone },
        { "console", DeviceType.Console },
        { "tv", DeviceType.Television },
        { "car browser", DeviceType.CarBrowser },
        { "smart display", DeviceType.SmartDisplay },
        { "camera", DeviceType.Camera },
        { "portable media player", DeviceType.PortableMediaPlayer },
        { "phablet", DeviceType.Phablet },
        { "smart speaker", DeviceType.SmartSpeaker },
        { "wearable", DeviceType.Wearable },
        { "peripheral", DeviceType.Peripheral },
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, BotCategory> BotCategoryMapping =
        new Dictionary<string, BotCategory>().ToFrozenDictionary();

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        RespectRequiredConstructorParameters = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new RegexJsonConverter() },
    };

    public static void ConvertDeviceRegex()
    {
        var entries = YamlLoader.LoadDictionary<DeviceYaml>(
            Path.Combine(BaseDirectory, DevicesFile + ".yml")
        );

        var result = entries.Select(x => new Device
        {
            Brand = x.Key,
            Regex = x.Value.Regex,
            Type = x.Value.Type is null ? null : DeviceTypeMapping[x.Value.Type],
            Model = x.Value.Model,
            ModelVariants = x
                .Value.ModelVariants?.Select(y => new DeviceModel
                {
                    Regex = y.Regex,
                    Type = y.Type is null ? null : DeviceTypeMapping[y.Type],
                    Brand = y.Brand,
                    Name = y.Name,
                })
                .ToList(),
        });

        File.WriteAllText(
            DevicesFile + ".json",
            JsonSerializer.Serialize(result, JsonSerializerOptions)
        );
    }

    public static void ConvertCollectionFixture()
    {
        var entries = YamlLoader.LoadList<UserAgentFixtureYaml>(
            Path.Combine(BaseDirectory, CollectionFile + ".yml")
        );

        var result = entries.Select(x => new UserAgentFixture
        {
            UserAgent = x.UserAgent,
            Headers = x.Headers,
            Os = x.Os is null
                ? null
                : new OsInfo
                {
                    Name = x.Os.Name,
                    Code = OsParser.OsNameMapping[x.Os.Name],
                    Version = x.Os.Version,
                    CpuArchitecture = x.Os.Platform,
                    Family = x.OsFamily == "Unknown" ? null : x.OsFamily,
                },
            Client =
                x.Client is null || x.Client.Type == "browser"
                    ? null
                    : new ClientInfo
                    {
                        Name = x.Client.Name,
                        Type = ClientTypeMapping[x.Client.Type],
                        Version = x.Client.Version,
                    },
            Browser =
                x.Client is null || x.Client.Type != "browser"
                    ? null
                    : new BrowserInfo
                    {
                        Name = x.Client.Name,
                        Code = BrowserParser.BrowserNameMapping[x.Client.Name],
                        Version = x.Client.Version,
                        Family = x.BrowserFamily == "Unknown" ? null : x.BrowserFamily,
                        Engine =
                            x.Client.Engine is null && x.Client.EngineVersion is null
                                ? null
                                : new EngineInfo
                                {
                                    Name = x.Client.Engine,
                                    Version = x.Client.EngineVersion,
                                },
                    },
            Device =
                x.Device is null
                || (x.Device.Brand is null && x.Device.Model is null && x.Device.Type is null)
                    ? null
                    : new DeviceInfo
                    {
                        Type = x.Device.Type is null ? null : DeviceTypeMapping[x.Device.Type],
                        Brand = x.Device.Brand is null or "Unknown"
                            ? null
                            : new BrandInfo
                            {
                                Name = x.Device.Brand,
                                Code = DeviceParserBase.BrandNameMapping[x.Device.Brand],
                            },
                        Model = x.Device.Model,
                    },
            Bot = x.Bot is null
                ? null
                : new BotInfo
                {
                    Name = x.Bot.Name,
                    Category = x.Bot.Category is null ? null : BotCategoryMapping[x.Bot.Category],
                    Url = x.Bot.Url,
                    Producer =
                        x.Bot.Producer is null
                        || (x.Bot.Producer.Name is null && x.Bot.Producer.Url is null)
                            ? null
                            : new ProducerInfo
                            {
                                Name = x.Bot.Producer.Name,
                                Url = x.Bot.Producer.Url,
                            },
                },
        });

        File.WriteAllText(
            CollectionFile + ".json",
            JsonSerializer.Serialize(result, JsonSerializerOptions)
        );
    }
}
