using System.Collections.Frozen;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using UaDetector.Abstractions.Enums;
using UaDetector.Abstractions.Models;
using UaDetector.Models;
using UaDetector.Registries;
using UaDetector.Tests.Fixtures.Models;
using UaDetector.YamlJsonConverter.Fixtures;
using UaDetector.YamlJsonConverter.Models;
using UaDetector.YamlJsonConverter.Utils;
using Os = UaDetector.Models.Os;

namespace UaDetector.YamlJsonConverter;

public static class YamlToJsonConverter
{
    private const string BaseDirectory = "Inputs";

    private const string BrowsersFile = "browsers";
    private const string ClientsFile = "clients";
    private const string OsFile = "os";
    private const string DevicesFile = "devices";
    private const string BotsFile = "bots";

    private const string CollectionFixturesFile = "collection_fixtures";
    private const string OsFixturesFile = "os_fixtures";

    private static readonly FrozenDictionary<string, ClientType> ClientTypeMappings =
        new Dictionary<string, ClientType>
        {
            { "mobile app", ClientType.MobileApp },
            { "mediaplayer", ClientType.MediaPlayer },
            { "library", ClientType.Library },
            { "feed reader", ClientType.FeedReader },
            { "pim", ClientType.PersonalInformationManager },
        }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, DeviceType> DeviceTypeMappings =
        new Dictionary<string, DeviceType>
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

    private static readonly FrozenDictionary<string, BotCategory> BotCategoryMappings =
        new Dictionary<string, BotCategory>
        {
            { "Search bot", BotCategory.SearchBot },
            { "Search tools", BotCategory.SearchTools },
            { "Security search bot", BotCategory.SecuritySearchBot },
            { "Crawler", BotCategory.Crawler },
            { "Validator", BotCategory.Validator },
            { "Security Checker", BotCategory.SecurityChecker },
            { "Feed Fetcher", BotCategory.FeedFetcher },
            { "Feed Reader", BotCategory.FeedReader },
            { "Feed Parser", BotCategory.FeedParser },
            { "Site Monitor", BotCategory.SiteMonitor },
            { "Network Monitor", BotCategory.NetworkMonitor },
            { "Service Agent", BotCategory.ServiceAgent },
            { "Service bot", BotCategory.ServiceBot },
            { "Social Media Agent", BotCategory.SocialMediaAgent },
            { "Read-it-later Service", BotCategory.ReadItLaterService },
            { "Benchmark", BotCategory.Benchmark },
        }.ToFrozenDictionary();

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        RespectRequiredConstructorParameters = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new RegexJsonConverter() },
    };

    public static void ConvertBrowserRegex()
    {
        var entries = YamlLoader.LoadList<BrowserYaml>(
            Path.Combine(BaseDirectory, BrowsersFile + ".yml")
        );

        var result = entries.Select(x => new Browser
        {
            Regex = new Regex(x.Regex),
            Name = x.Name,
            Version = x.Version,
            Engine =
                x.Engine is null
                || (
                    string.IsNullOrEmpty(x.Engine.Default)
                    && (x.Engine.Versions is null || x.Engine.Versions.Count == 0)
                )
                    ? null
                    : new BrowserEngine
                    {
                        Default = string.IsNullOrEmpty(x.Engine.Default) ? null : x.Engine.Default,
                        Versions =
                            x.Engine.Versions == null
                            || x.Engine.Versions.Values.All(string.IsNullOrEmpty)
                                ? null
                                : x
                                    .Engine.Versions?.Where(kvp => !string.IsNullOrEmpty(kvp.Value))
                                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                    },
        });

        File.WriteAllText(
            BrowsersFile + ".json",
            JsonSerializer.Serialize(result, JsonSerializerOptions)
        );
    }

    public static void ConvertClientRegex()
    {
        var entries = YamlLoader.LoadList<ClientYaml>(
            Path.Combine(BaseDirectory, ClientsFile + ".yml")
        );

        var result = entries.Select(x => new Client
        {
            Regex = new Regex(x.Regex),
            Name = x.Name,
            Version = x.Version,
        });

        File.WriteAllText(
            ClientsFile + ".json",
            JsonSerializer.Serialize(result, JsonSerializerOptions)
        );
    }

    public static void ConvertDeviceRegex()
    {
        var entries = YamlLoader.LoadDictionary<DeviceYaml>(
            Path.Combine(BaseDirectory, DevicesFile + ".yml")
        );

        var result = entries.Select(x => new Device
        {
            Brand = x.Key,
            Regex = x.Value.Regex,
            Type = x.Value.Type is null ? null : DeviceTypeMappings[x.Value.Type],
            Model = x.Value.Model,
            ModelVariants = x
                .Value.ModelVariants?.Select(y => new DeviceModel
                {
                    Regex = y.Regex,
                    Type = y.Type is null ? null : DeviceTypeMappings[y.Type],
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

    public static void ConvertBotRegex()
    {
        var entries = YamlLoader.LoadList<BotYaml>(Path.Combine(BaseDirectory, BotsFile + ".yml"));

        var result = entries.Select(x => new Bot
        {
            Regex = x.Regex,
            Name = x.Name,
            Category = x.Category is null ? null : BotCategoryMappings[x.Category],
            Url = x.Url,
            Producer = x.Producer is null || (x.Producer.Name is null && x.Producer.Url is null)
                ? null
                : new BotProducer { Name = x.Producer.Name, Url = x.Producer.Url },
        });

        File.WriteAllText(
            BotsFile + ".json",
            JsonSerializer.Serialize(result, JsonSerializerOptions)
        );
    }

    public static void ConvertOsRegex()
    {
        var entries = YamlLoader.LoadList<OsYaml>(Path.Combine(BaseDirectory, OsFile + ".yml"));

        var result = entries.Select(x => new Os
        {
            Regex = x.Regex,
            Name = x.Name,
            Version = x.Version,
            Versions = x
                .Versions?.Select(item => new OsVersion
                {
                    Regex = item.Regex,
                    Version = item.Version,
                })
                .ToList(),
        });

        File.WriteAllText(
            OsFile + ".json",
            JsonSerializer.Serialize(result, JsonSerializerOptions)
        );
    }

    public static void ConvertOsFixture()
    {
        var entries = YamlLoader.LoadList<OsFixtureYaml>(
            Path.Combine(BaseDirectory, OsFixturesFile + ".yml")
        );

        var result = entries.Select(x => new OsFixture
        {
            UserAgent = x.UserAgent,
            Os = new OsInfo
            {
                Name = x.Os.Name,
                Code = OsRegistry.OsNameMappings[x.Os.Name],
                Version = x.Os.Version,
                CpuArchitecture = x.Os.Platform,
                Family = x.Os.Family,
            },
        });

        File.WriteAllText(
            OsFixturesFile + ".json",
            JsonSerializer.Serialize(result, JsonSerializerOptions)
        );
    }

    public static void ConvertCollectionFixture()
    {
        var entries = YamlLoader.LoadList<UserAgentFixtureYaml>(
            Path.Combine(BaseDirectory, CollectionFixturesFile + ".yml")
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
                    Code = OsRegistry.OsNameMappings[x.Os.Name],
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
                        Type = ClientTypeMappings[x.Client.Type],
                        Version = x.Client.Version,
                    },
            Browser =
                x.Client is null || x.Client.Type != "browser"
                    ? null
                    : new BrowserInfo
                    {
                        Name = x.Client.Name,
                        Code = BrowserRegistry.BrowserNameMappings[x.Client.Name],
                        Version = x.Client.Version,
                        Family =
                            x.BrowserFamily == "Unknown"
                                ? null
                                : x.BrowserFamily ?? x.Client.Family,
                        Engine =
                            string.IsNullOrEmpty(x.Client.Engine)
                            && string.IsNullOrEmpty(x.Client.EngineVersion)
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
                        Type = x.Device.Type is null ? null : DeviceTypeMappings[x.Device.Type],
                        Brand = x.Device.Brand is null or "Unknown"
                            ? null
                            : new BrandInfo
                            {
                                Name = x.Device.Brand,
                                Code = BrandRegistry.BrandNameMappings[x.Device.Brand],
                            },
                        Model = x.Device.Model,
                    },
            Bot = x.Bot is null
                ? null
                : new BotInfo
                {
                    Name = x.Bot.Name,
                    Category = x.Bot.Category is null ? null : BotCategoryMappings[x.Bot.Category],
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
            CollectionFixturesFile + ".json",
            JsonSerializer.Serialize(result, JsonSerializerOptions)
        );
    }
}
