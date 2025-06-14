using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Models.Constants;
using UaDetector.Models.Enums;
using UaDetector.Parsers;
using UaDetector.Parsers.Devices;
using UaDetector.Results;

namespace UaDetector;

public sealed class UaDetector : IUaDetector
{
    private const string CacheKeyPrefix = "ua";
    private readonly IUaDetectorCache? _cache;
    private readonly UaDetectorOptions _uaDetectorOptions;
    private readonly OsParser _osParser;
    private readonly BrowserParser _browserParser;
    private readonly ClientParser _clientParser;
    private readonly BotParser _botParser;
    private static readonly Regex ContainsLetterRegex;
    private static readonly Regex AndroidVrFragment;
    private static readonly Regex ChromeRegex;
    private static readonly Regex MobileRegex;
    private static readonly Regex PadRegex;
    private static readonly Regex OperaTabletRegex;
    private static readonly Regex OperaTvStoreRegex;
    private static readonly Regex TouchEnabledRegex;
    private static readonly Regex TizenOrSmartTvRegex;
    private static readonly Regex TvFragmentRegex;
    private static readonly Regex AndroidTabletFragmentRegex;
    private static readonly Regex AndroidMobileFragmentRegex;
    private static readonly Regex PuffinSecureBrowserDesktopRegex;
    private static readonly Regex PuffinWebBrowserSmartphoneRegex;
    private static readonly Regex PuffinWebBrowserTabletRegex;
    private static readonly Regex AndroidRegex;
    private static readonly Regex DesktopFragment;
    private static readonly FrozenSet<string> AppleOsNames;
    private static readonly FrozenSet<string> TvBrowsers;
    private static readonly FrozenSet<string> TvClients;
    private static readonly List<KeyValuePair<string, DeviceType>> ClientHintFormFactorsMapping;

    private readonly IReadOnlyList<DeviceParserBase> _deviceParsers =
    [
        new HbbTvParser(),
        new ShellTvParser(),
        new NotebookParser(),
        new ConsoleParser(),
        new CarBrowserParser(),
        new CameraParser(),
        new PortableMediaPlayerParser(),
        new MobileParser(),
    ];

    static UaDetector()
    {
        ContainsLetterRegex = new Regex("[a-zA-Z]", RegexOptions.Compiled);
        AndroidVrFragment = BuildRegex("Android( [.0-9]+)?; Mobile VR;| VR ");
        ChromeRegex = BuildRegex("Chrome/[.0-9]*");
        MobileRegex = BuildRegex("(?:Mobile|eliboM)");
        PadRegex = BuildRegex("Pad/APad");
        OperaTabletRegex = BuildRegex("Opera Tablet");
        OperaTvStoreRegex = BuildRegex("Opera TV Store| OMI/");
        TouchEnabledRegex = BuildRegex("Touch");
        TizenOrSmartTvRegex = BuildRegex("SmartTV|Tizen.+ TV .+$");
        TvFragmentRegex = BuildRegex(@"\(TV;");
        AndroidTabletFragmentRegex = BuildRegex(
            @"Android( [.0-9]+)?; Tablet;|Tablet(?! PC)|.*\-tablet$"
        );
        AndroidMobileFragmentRegex = BuildRegex(@"Android( [.0-9]+)?; Mobile;|.*\-mobile$");
        PuffinSecureBrowserDesktopRegex = BuildRegex(@"Puffin/(?:\d+[.\d]+)[LMW]D");
        PuffinWebBrowserSmartphoneRegex = BuildRegex(@"Puffin/(?:\d+[.\d]+)[AIFLW]P");
        PuffinWebBrowserTabletRegex = BuildRegex(@"Puffin/(?:\d+[.\d]+)[AILW]T");
        AndroidRegex = BuildRegex(
            @"Andr0id|(?:Android(?: UHD)?|Google) TV|\(lite\) TV|BRAVIA|Firebolt| TV$"
        );
        DesktopFragment = BuildRegex("Desktop(?: (x(?:32|64)|WOW64))?;");

        AppleOsNames = new[]
        {
            OsNames.IPadOs,
            OsNames.TvOs,
            OsNames.WatchOs,
            OsNames.IOs,
            OsNames.Mac,
        }.ToFrozenSet();

        TvBrowsers = new[]
        {
            BrowserNames.Kylo,
            BrowserNames.EspialTvBrowser,
            BrowserNames.LujoTvBrowser,
            BrowserNames.LogicUiTvBrowser,
            BrowserNames.OpenTvBrowser,
            BrowserNames.SeraphicSraf,
            BrowserNames.OperaDevices,
            BrowserNames.CrowBrowser,
            BrowserNames.VewdBrowser,
            BrowserNames.QuickSearchTv,
            BrowserNames.QjyTvBrowser,
            BrowserNames.TvBro,
        }.ToFrozenSet();

        TvClients = new[] { "TiviMate" }.ToFrozenSet();

        ClientHintFormFactorsMapping =
        [
            new KeyValuePair<string, DeviceType>("automotive", DeviceType.CarBrowser),
            new KeyValuePair<string, DeviceType>("xr", DeviceType.Wearable),
            new KeyValuePair<string, DeviceType>("watch", DeviceType.Wearable),
            new KeyValuePair<string, DeviceType>("mobile", DeviceType.Smartphone),
            new KeyValuePair<string, DeviceType>("tablet", DeviceType.Tablet),
            new KeyValuePair<string, DeviceType>("desktop", DeviceType.Desktop),
            new KeyValuePair<string, DeviceType>("eink", DeviceType.Tablet),
        ];
    }

    public UaDetector(UaDetectorOptions? uaDetectorOptions = null)
    {
        _uaDetectorOptions = uaDetectorOptions ?? new UaDetectorOptions();
        _cache = uaDetectorOptions?.Cache;
        _osParser = new OsParser(uaDetectorOptions);
        _browserParser = new BrowserParser(uaDetectorOptions);
        _clientParser = new ClientParser(uaDetectorOptions);
        _botParser = new BotParser(new BotParserOptions { Cache = _cache });
    }

    private static Regex BuildRegex(string pattern)
    {
        return new Regex(
            $"(?:^|[^A-Z_-])(?:{pattern})",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );
    }

    private static bool IsWindows8OrLater(OsInfo os)
    {
        return os is { Name: OsNames.Windows, Version.Length: > 0 }
            && ParserExtensions.TryCompareVersions(os.Version, "8", out var comparisonResult)
            && comparisonResult >= 0;
    }

    private static bool IsDesktop(OsInfo? os, BrowserInfo? browser)
    {
        if (browser is not null && BrowserParser.MobileOnlyBrowsers.Contains(browser.Code))
        {
            return false;
        }

        return os?.Family?.Length > 0 && OsParser.DesktopOsFamilies.Contains(os.Family);
    }

    private static bool TryParseDeviceFromClientHints(
        ClientHints clientHints,
        [NotNullWhen(true)] out ClientHintsDeviceInfo? result
    )
    {
        if (clientHints.Model is null or { Length: 0 })
        {
            result = null;
            return false;
        }

        DeviceType? deviceType = null;

        foreach (var formFactor in ClientHintFormFactorsMapping)
        {
            if (clientHints.FormFactors.Contains(formFactor.Key))
            {
                deviceType = formFactor.Value;
                break;
            }
        }

        result = new ClientHintsDeviceInfo { Type = deviceType, Model = clientHints.Model };
        return true;
    }

    private bool TryParseDevice(
        string userAgent,
        ClientHints clientHints,
        OsInfo? os,
        BrowserInfo? browser,
        ClientInfo? client,
        [NotNullWhen(true)] out DeviceInfo? result
    )
    {
        DeviceType? deviceType = null;
        string? model = null;
        string? brand = null;

        if (TryParseDeviceFromClientHints(clientHints, out var deviceFromClientHints))
        {
            deviceType = deviceFromClientHints.Type;
            model = deviceFromClientHints.Model;
        }

        if (
            model?.Length > 0
            || (
                !ParserExtensions.HasUserAgentClientHintsFragment(userAgent)
                && !ParserExtensions.HasUserAgentDesktopFragment(userAgent)
            )
        )
        {
            foreach (var parser in _deviceParsers)
            {
                if (parser.TryParse(userAgent, out var deviceInfo))
                {
                    deviceType = deviceInfo.Type;
                    model = deviceInfo.Model;
                    brand = deviceInfo.Brand;
                    break;
                }
            }
        }

        // If the user agent does not specify a model, use the one from client hints.
        if (model is null or { Length: 0 } && clientHints.Model?.Length > 0)
        {
            model = clientHints.Model;
        }

        if (brand is null or { Length: 0 })
        {
            VendorFragmentParser.TryParseBrand(userAgent, out brand);
        }

        // Prevent misidentification of spoofed user agent as legitimate Apple.
        if (brand == BrandNames.Apple && os is not null && !AppleOsNames.Contains(os.Name))
        {
            deviceType = null;
            brand = null;
            model = null;
        }

        // Assume all devices running iOS or macOS are manufactured by Apple.
        if (brand is null or { Length: 0 } && os is not null && AppleOsNames.Contains(os.Name))
        {
            brand = BrandNames.Apple;
        }

        // User agents containing the fragment 'VR' are assumed to represent wearables.
        if (deviceType is null && AndroidVrFragment.IsMatch(userAgent))
        {
            deviceType = DeviceType.Wearable;
        }

        // Chrome on Android uses the 'Mobile' keyword in the user agent to indicate device type.
        // If 'Mobile' is present, the device is considered a smartphone; otherwise, it's a tablet.
        //
        // Note: The browser family is not checked, as some mobile apps may use Chrome without a detected browser.
        // Instead, the user agent is directly checked for the presence of 'Chrome'.
        if (
            deviceType is null
            && os?.Family == OsFamilies.Android
            && ChromeRegex.IsMatch(userAgent)
        )
        {
            deviceType = MobileRegex.IsMatch(userAgent) ? DeviceType.Smartphone : DeviceType.Tablet;
        }

        // User agents containing the fragment 'Pad' or 'APad' are assumed to represent tablets.
        if (deviceType == DeviceType.Smartphone && PadRegex.IsMatch(userAgent))
        {
            deviceType = DeviceType.Tablet;
        }

        // User agents containing the fragments 'Android; Tablet;' or 'Opera Tablet' are assumed to represent tablets.
        if (
            deviceType is null
            && (
                AndroidTabletFragmentRegex.IsMatch(userAgent) || OperaTabletRegex.IsMatch(userAgent)
            )
        )
        {
            deviceType = DeviceType.Tablet;
        }

        // User agents containing the fragment 'Android; Mobile;' are assumed to represent smartphones.
        if (deviceType is null && AndroidMobileFragmentRegex.IsMatch(userAgent))
        {
            deviceType = DeviceType.Smartphone;
        }

        // Android versions up to 3.0 were designed exclusively for smartphones.
        // However, due to the late release of 3.0 (which was tablet-only), many tablets were running Android 2.x.
        // Starting with Android 4.0, the platform was unified, supporting both smartphones and tablets.
        //
        // Therefore, it is expected that:
        // - Devices running Android versions earlier than 2.0 are smartphones.
        // - Devices running Android 3.x are tablets.
        // - Devices running Android 2.x and 4.x+ have an unknown device type.
        if (deviceType is null && os is { Name: OsNames.Android, Version.Length: > 0 })
        {
            if (
                ParserExtensions.TryCompareVersions(os.Version, "2.0", out var comparisonResult)
                && comparisonResult == -1
            )
            {
                deviceType = DeviceType.Smartphone;
            }
            else if (
                ParserExtensions.TryCompareVersions(os.Version, "3.0", out comparisonResult)
                && comparisonResult >= 0
                && ParserExtensions.TryCompareVersions(os.Version, "4.0", out comparisonResult)
                && comparisonResult == -1
            )
            {
                deviceType = DeviceType.Tablet;
            }
        }

        // Android feature phones are likely to be smartphones.
        if (deviceType == DeviceType.FeaturePhone && os?.Family == OsFamilies.Android)
        {
            deviceType = DeviceType.Smartphone;
        }

        // Unknown devices running Java ME are likely feature phones.
        if (deviceType is null && os?.Name == OsNames.JavaMe)
        {
            deviceType = DeviceType.FeaturePhone;
        }

        // Devices running KaiOS are likely to be feature phones.
        if (os?.Name == OsNames.KaiOs)
        {
            deviceType = DeviceType.FeaturePhone;
        }

        // According to http://msdn.microsoft.com/en-us/library/ie/hh920767(v=vs.85).aspx,
        // Internet Explorer 10 introduces the "Touch" UA string token. If this token appears at the end of the
        // UA string, the device has touch capabilities and is running Windows 8 (or later).
        // This token is transmitted by touch-enabled systems running Windows 8 (or Windows RT).
        //
        // Since most touch-enabled devices are tablets, with desktops and notebooks being the exception,
        // it is assumed that all Windows 8 touch devices are tablets.
        if (
            deviceType is null
            && os is not null
            && (os.Name == OsNames.WindowsRt || IsWindows8OrLater(os))
            && TouchEnabledRegex.IsMatch(userAgent)
        )
        {
            deviceType = DeviceType.Tablet;
        }

        // Devices running Puffin Secure Browser that include the letter 'D' are assumed to be desktops.
        if (deviceType is null && PuffinSecureBrowserDesktopRegex.IsMatch(userAgent))
        {
            deviceType = DeviceType.Desktop;
        }

        // Devices running Puffin Web Browser that include the letter 'P' are assumed to be smartphones.
        if (deviceType is null && PuffinWebBrowserSmartphoneRegex.IsMatch(userAgent))
        {
            deviceType = DeviceType.Smartphone;
        }

        // Devices running Puffin Web Browser that include the letter 'T' are assumed to be smartphones.
        if (deviceType is null && PuffinWebBrowserTabletRegex.IsMatch(userAgent))
        {
            deviceType = DeviceType.Tablet;
        }

        // Devices running Opera TV Store are assumed to be TVs.
        if (OperaTvStoreRegex.IsMatch(userAgent))
        {
            deviceType = DeviceType.Television;
        }

        // Devices running Coolita OS are assumed to be TVs.
        if (os?.Name == OsNames.CoolitaOs)
        {
            deviceType = DeviceType.Television;
            brand = BrandNames.Coocaa;
        }

        // Devices containing "Andr0id" in the user agent string are assumed to be TVs.
        if (
            deviceType != DeviceType.Television
            && deviceType != DeviceType.Peripheral
            && AndroidRegex.IsMatch(userAgent)
        )
        {
            deviceType = DeviceType.Television;
        }

        // Devices using these clients are assumed to be TVs.
        if (
            browser is not null && TvBrowsers.Contains(browser.Name)
            || client is not null && TvClients.Contains(client.Name)
        )
        {
            deviceType = DeviceType.Television;
        }

        // User agents containing the "TV" fragment are assumed to be TVs.
        if (deviceType is null && TvFragmentRegex.IsMatch(userAgent))
        {
            deviceType = DeviceType.Television;
        }

        // Devices running Tizen TV or SmartTV are assumed to be TVs.
        if (deviceType is null && TizenOrSmartTvRegex.IsMatch(userAgent))
        {
            deviceType = DeviceType.Television;
        }

        // User agents containing the "Desktop" fragment are assumed to be desktops.
        if (
            deviceType != DeviceType.Desktop
            && userAgent.Contains("Desktop")
            && DesktopFragment.IsMatch(userAgent)
        )
        {
            deviceType = DeviceType.Desktop;
        }

        if (deviceType is null && IsDesktop(os, browser))
        {
            deviceType = DeviceType.Desktop;
        }

        if (deviceType is null && model is null && brand is null)
        {
            result = null;
        }
        else
        {
            result = new DeviceInfo
            {
                Type = deviceType,
                Model = model,
                Brand =
                    brand is not null
                    && DeviceParserBase.BrandNameMapping.TryGetValue(brand, out var brandCode)
                        ? new BrandInfo { Name = brand, Code = brandCode }
                        : null,
            };
        }

        return result is not null;
    }

    public bool TryParse(string userAgent, [NotNullWhen(true)] out UserAgentInfo? result)
    {
        return TryParse(userAgent, ImmutableDictionary<string, string?>.Empty, out result);
    }

    public bool TryParse(
        string userAgent,
        IDictionary<string, string?> headers,
        [NotNullWhen(true)] out UserAgentInfo? result
    )
    {
        if (
            (userAgent.Length == 0 || !ContainsLetterRegex.IsMatch(userAgent))
            && headers.Count == 0
        )
        {
            result = null;
            return false;
        }

        var clientHints = ClientHints.Create(headers);

        if (ParserExtensions.TryRestoreUserAgent(userAgent, clientHints, out var restoredUserAgent))
        {
            userAgent = restoredUserAgent;
        }

        var cacheKey = $"{CacheKeyPrefix}:{userAgent}";

        if (_cache is not null && _cache.TryGet(cacheKey, out result))
        {
            return result is not null;
        }

        if (
            !_uaDetectorOptions.DisableBotDetection
            && _botParser.TryParse(userAgent, out BotInfo? bot)
        )
        {
            result = new UserAgentInfo
            {
                Bot = bot,
                Os = null,
                Browser = null,
                Client = null,
                Device = null,
            };

            _cache?.Set(cacheKey, result);
            return true;
        }

        BrowserInfo? browser = null;

        _osParser.TryParse(userAgent, clientHints, out var os);

        if (!_clientParser.TryParse(userAgent, clientHints, out ClientInfo? client))
        {
            _browserParser.TryParse(userAgent, clientHints, out browser);
        }

        TryParseDevice(userAgent, clientHints, os, browser, client, out var device);

        if (os is null && browser is null && client is null && device is null)
        {
            result = null;
        }
        else
        {
            result = new UserAgentInfo
            {
                Os = os,
                Browser = browser,
                Client = client,
                Device = device,
                Bot = null,
            };
        }

        _cache?.Set(cacheKey, result);
        return result is not null;
    }

    private sealed class ClientHintsDeviceInfo
    {
        public required DeviceType? Type { get; init; }
        public required string Model { get; init; }
    }
}
