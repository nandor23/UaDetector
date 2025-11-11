using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Abstractions.Constants;
using UaDetector.Abstractions.Enums;
using UaDetector.Abstractions.Models;
using UaDetector.Attributes;
using UaDetector.Models;
using UaDetector.Parsers.Browsers;
using UaDetector.Registries;
using UaDetector.Utilities;

namespace UaDetector.Parsers;

public sealed partial class BrowserParser : IBrowserParser
{
    [RegexSource("Resources/Browsers/browsers.json")]
    internal static partial IReadOnlyList<Browser> Browsers { get; }

    private const string CacheKeyPrefix = "browser";
    private readonly IUaDetectorCache? _cache;
    private readonly UaDetectorOptions _uaDetectorOptions;
    private readonly ClientParser _clientParser;
    private readonly BotParser _botParser;

    internal static readonly FrozenDictionary<string, string> CompactToFullNameMappings;
    internal static readonly FrozenSet<BrowserCode> MobileOnlyBrowsers;
    private static readonly FrozenDictionary<string, FrozenSet<BrowserCode>> BrowserFamilyMappings;
    private static readonly FrozenDictionary<string, FrozenSet<string>> ClientHintBrandMappings;
    private static readonly FrozenSet<BrowserCode> PriorityBrowsers;
    private static readonly FrozenSet<BrowserCode> ChromiumBrowsers;
    private static readonly Regex ChromeSafariRegex;
    private static readonly Regex CypressOrPhantomJsRegex;
    private static readonly Regex IridiumVersionRegex;

    static BrowserParser()
    {
        var duplicateCompactNames = BrowserRegistry
            .BrowserCodeMappings.Values.Select(x => x.RemoveSpaces())
            .GroupBy(x => x)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        var mappings = new Dictionary<string, string>();

        foreach (var name in BrowserRegistry.BrowserNameMappings.Keys)
        {
            var compactName = name.RemoveSpaces();

            if (!duplicateCompactNames.Contains(compactName))
            {
                mappings.Add(compactName, name);
            }
        }

        CompactToFullNameMappings = mappings.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

        BrowserFamilyMappings = new Dictionary<string, FrozenSet<BrowserCode>>
        {
            { BrowserFamilies.AndroidBrowser, new[] { BrowserCode.AndroidBrowser }.ToFrozenSet() },
            {
                BrowserFamilies.BlackBerryBrowser,
                new[] { BrowserCode.BlackBerryBrowser }.ToFrozenSet()
            },
            {
                BrowserFamilies.Baidu,
                new[]
                {
                    BrowserCode.BaiduBrowser,
                    BrowserCode.BaiduSpark,
                    BrowserCode.HonorBrowser,
                }.ToFrozenSet()
            },
            {
                BrowserFamilies.Amiga,
                new[] { BrowserCode.AmigaVoyager, BrowserCode.AmigaAweb }.ToFrozenSet()
            },
            {
                BrowserFamilies.Chrome,
                new[]
                {
                    BrowserCode.Chrome,
                    BrowserCode.Browser2345,
                    BrowserCode.SevenStar,
                    BrowserCode.Atom,
                    BrowserCode.AviraSecureBrowser,
                    BrowserCode.AolShieldPro,
                    BrowserCode.AolDesktop,
                    BrowserCode.AlohaBrowserLite,
                    BrowserCode.Arvin,
                    BrowserCode.Amigo,
                    BrowserCode.AvastSecureBrowser,
                    BrowserCode.BeakerBrowser,
                    BrowserCode.Beamrise,
                    BrowserCode.Brave,
                    BrowserCode.Colibri,
                    BrowserCode.ChimLac,
                    BrowserCode.ChromiumGost,
                    BrowserCode.CoolBrowser,
                    BrowserCode.Chedot,
                    BrowserCode.CocCoc,
                    BrowserCode.ComodoDragon,
                    BrowserCode.CmBrowser,
                    BrowserCode.ChromeFrame,
                    BrowserCode.CravingExplorer,
                    BrowserCode.Browser115,
                    BrowserCode.ChromeMobileIos,
                    BrowserCode.CCleaner,
                    BrowserCode.ChromeMobile,
                    BrowserCode.CoolNovo,
                    BrowserCode.ChromePlus,
                    BrowserCode.Chromium,
                    BrowserCode.ChromeWebview,
                    BrowserCode.Cornowser,
                    BrowserCode.Deledao,
                    BrowserCode.DuckDuckGoPrivacyBrowser,
                    BrowserCode.DegDegan,
                    BrowserCode.DotBrowser,
                    BrowserCode.Ecosia,
                    BrowserCode.ElementsBrowser,
                    BrowserCode.EuiBrowser,
                    BrowserCode.EdgeWebView,
                    BrowserCode.Falkon,
                    BrowserCode.Flast,
                    BrowserCode.GlassBrowser,
                    BrowserCode.GinxDroidBrowser,
                    BrowserCode.OceanHero,
                    BrowserCode.HawkTurboBrowser,
                    BrowserCode.Helio,
                    BrowserCode.OhHaiBrowser,
                    BrowserCode.HasBrowser,
                    BrowserCode.IronMobile,
                    BrowserCode.Iron,
                    BrowserCode.JapanBrowser,
                    BrowserCode.Kinza,
                    BrowserCode.Kiwi,
                    BrowserCode.LieBaoFast,
                    BrowserCode.Lulumi,
                    BrowserCode.LovenseBrowser,
                    BrowserCode.MCent,
                    BrowserCode.Maelstrom,
                    BrowserCode.Mandarin,
                    BrowserCode.MonumentBrowser,
                    BrowserCode.MobileSilk,
                    BrowserCode.MintBrowser,
                    BrowserCode.MeizuBrowser,
                    BrowserCode.MxNitro,
                    BrowserCode.NfsBrowser,
                    BrowserCode.OriginInGameOverlay,
                    BrowserCode.Odin,
                    BrowserCode.OpenFin,
                    BrowserCode.OculusBrowser,
                    BrowserCode.PhoenixBrowser,
                    BrowserCode.Polarity,
                    BrowserCode.Quark,
                    BrowserCode.QtWebEngine,
                    BrowserCode.RockMelt,
                    BrowserCode.SteamInGameOverlay,
                    BrowserCode.Slimjet,
                    BrowserCode.SeewoBrowser,
                    BrowserCode.SecureBrowser,
                    BrowserCode.SamsungBrowser,
                    BrowserCode.Stargon,
                    BrowserCode.SeraphicSraf,
                    BrowserCode.SuperFastBrowser,
                    BrowserCode.SFive,
                    BrowserCode.SalamWeb,
                    BrowserCode.Sizzy,
                    BrowserCode.SeznamBrowser,
                    BrowserCode.StampyBrowser,
                    BrowserCode.TaoBrowser,
                    BrowserCode.TentaBrowser,
                    BrowserCode.ToGate,
                    BrowserCode.TBrowser2,
                    BrowserCode.TweakStyle,
                    BrowserCode.Tungsten,
                    BrowserCode.TvBro,
                    BrowserCode.UBrowser2,
                    BrowserCode.UrBrowser,
                    BrowserCode.VenusBrowser,
                    BrowserCode.AvgSecureBrowser,
                    BrowserCode.Vivaldi,
                    BrowserCode.VMwareAirWatch,
                    BrowserCode.WebExplorer,
                    BrowserCode.WhaleBrowser,
                    BrowserCode.Xvast,
                    BrowserCode.YahooJapanBrowser,
                    BrowserCode.FlashBrowser,
                    BrowserCode.SpectreBrowser,
                    BrowserCode.Bonsai,
                    BrowserCode.HarmanBrowser,
                    BrowserCode.PsiSecureBrowser,
                    BrowserCode.LagatosBrowser,
                    BrowserCode.LtBrowser,
                    BrowserCode.PeepsDBrowser,
                    BrowserCode.SushiBrowser,
                    BrowserCode.HuaweiBrowserMobile,
                    BrowserCode.HuaweiBrowser,
                    BrowserCode.IBrowser,
                    BrowserCode.TBrowser,
                    BrowserCode.ChanjetCloud,
                    BrowserCode.HawkQuickBrowser,
                    BrowserCode.HiBrowser,
                    BrowserCode.ApnBrowser,
                    BrowserCode.AdBlockBrowser,
                    BrowserCode.YouCare,
                    BrowserCode.Decentr,
                    BrowserCode.Gener8,
                    BrowserCode.DeltaBrowser,
                    BrowserCode.ApusBrowser,
                    BrowserCode.AskCom,
                    BrowserCode.UiBrowserMini,
                    BrowserCode.SavySoda,
                    BrowserCode.SavannahBrowser,
                    BrowserCode.SurfBrowser,
                    BrowserCode.SoulBrowser,
                    BrowserCode.SotiSurf,
                    BrowserCode.LexiBrowser,
                    BrowserCode.SmartBrowser,
                    BrowserCode.BelvaBrowser,
                    BrowserCode.Lilo,
                    BrowserCode.FloatBrowser,
                    BrowserCode.KidsSafeBrowser,
                    BrowserCode.VBrowser,
                    BrowserCode.CgBrowser,
                    BrowserCode.AzkaBrowser,
                    BrowserCode.MmxBrowser,
                    BrowserCode.BitchuteBrowser,
                    BrowserCode.NovaVideoDownloaderPro,
                    BrowserCode.PronHubBrowser,
                    BrowserCode.FrostPlus,
                    BrowserCode.ViasatBrowser,
                    BrowserCode.DucBrowser,
                    BrowserCode.DesiBrowser,
                    BrowserCode.PhantomMe,
                    BrowserCode.OpenBrowser,
                    BrowserCode.SecurePrivateBrowser,
                    BrowserCode.HubBrowser,
                    BrowserCode.TeslaBrowser,
                    BrowserCode.WaveBrowser,
                    BrowserCode.Sidekick,
                    BrowserCode.PiBrowser,
                    BrowserCode.XoolooInternet,
                    BrowserCode.UBrowser,
                    BrowserCode.Bloket,
                    BrowserCode.VastBrowser,
                    BrowserCode.XVpn,
                    BrowserCode.NoxBrowser,
                    BrowserCode.OfficeBrowser,
                    BrowserCode.RabbitPrivateBrowser,
                    BrowserCode.Iridium,
                    BrowserCode.HolaBrowser,
                    BrowserCode.Amerigo,
                    BrowserCode.XBrowserProSuperFast,
                    BrowserCode.PrivacyBrowser18Plus,
                    BrowserCode.BeyondPrivateBrowser,
                    BrowserCode.BlackLionBrowser,
                    BrowserCode.TucMiniBrowser,
                    BrowserCode.AppBrowzer,
                    BrowserCode.SxBrowser,
                    BrowserCode.FieryBrowser,
                    BrowserCode.Yagi,
                    BrowserCode.NextWordBrowser,
                    BrowserCode.NakedBrowserPro,
                    BrowserCode.Browser1Dm,
                    BrowserCode.Browser1DmPlus,
                    BrowserCode.AdultBrowser,
                    BrowserCode.XnxBrowser,
                    BrowserCode.XtremeCast,
                    BrowserCode.XBrowserLite,
                    BrowserCode.SweetBrowser,
                    BrowserCode.HtcBrowser,
                    BrowserCode.BrowserHupPro,
                    BrowserCode.Flyperlink,
                    BrowserCode.BanglaBrowser,
                    BrowserCode.Wavebox,
                    BrowserCode.SoundyBrowser,
                    BrowserCode.HeadlessChrome,
                    BrowserCode.OddBrowser,
                    BrowserCode.Pawxy,
                    BrowserCode.LujoTvBrowser,
                    BrowserCode.LogicUiTvBrowser,
                    BrowserCode.OpenTvBrowser,
                    BrowserCode.NortonPrivateBrowser,
                    BrowserCode.Alva,
                    BrowserCode.PicoBrowser,
                    BrowserCode.RokuBrowser,
                    BrowserCode.WorldBrowser,
                    BrowserCode.EveryBrowser,
                    BrowserCode.InstaBrowser,
                    BrowserCode.VertexSurf,
                    BrowserCode.HollaWebBrowser,
                    BrowserCode.TorBrowser,
                    BrowserCode.MarsLabWebBrowser,
                    BrowserCode.SunflowerBrowser,
                    BrowserCode.CaveBrowser,
                    BrowserCode.ZordoBrowser,
                    BrowserCode.DarkBrowser,
                    BrowserCode.FreedomBrowser,
                    BrowserCode.CrowBrowser,
                    BrowserCode.VewdBrowser,
                    BrowserCode.PrivateInternetBrowser,
                    BrowserCode.Frost,
                    BrowserCode.AirfindSecureBrowser,
                    BrowserCode.SecureX,
                    BrowserCode.IncognitoBrowser,
                    BrowserCode.GodzillaBrowser,
                    BrowserCode.OceanBrowser,
                    BrowserCode.Qmamu,
                    BrowserCode.BfBrowser,
                    BrowserCode.BroKeepBrowser,
                    BrowserCode.ProxyBrowser,
                    BrowserCode.HotBrowser,
                    BrowserCode.VdBrowser,
                    BrowserCode.Skye,
                    BrowserCode.QuickSearchTv,
                    BrowserCode.GoBrowser2,
                    BrowserCode.RaspbianChromium,
                    BrowserCode.Wexond,
                    BrowserCode.Catsxp,
                    BrowserCode.IntuneManagedBrowser,
                    BrowserCode.Bang,
                    BrowserCode.SberBrowser,
                    BrowserCode.JioSphere,
                    BrowserCode.OnBrowserLite,
                    BrowserCode.LeganBrowser,
                    BrowserCode.WebDiscover,
                    BrowserCode.Qiyu,
                    BrowserCode.EastBrowser,
                    BrowserCode.LiriBrowser,
                    BrowserCode.SlimBoat,
                    BrowserCode.BasicWebBrowser,
                    BrowserCode.Kitt,
                    BrowserCode.WukongBrowser,
                    BrowserCode.TotalBrowser,
                    BrowserCode.Spark,
                    BrowserCode.MiBrowser,
                    BrowserCode.Presearch,
                    BrowserCode.Perk,
                    BrowserCode.Veera,
                    BrowserCode.PintarBrowser,
                    BrowserCode.BrowserMini,
                    BrowserCode.FossBrowser,
                    BrowserCode.PeachBrowser,
                    BrowserCode.AppTecSecureBrowser,
                    BrowserCode.OjrBrowser,
                    BrowserCode.Dezor,
                    BrowserCode.Tusk,
                    BrowserCode.PrivacyBrowser,
                    BrowserCode.ProxyFox,
                    BrowserCode.ProxyMax,
                    BrowserCode.KeepSolidBrowser,
                    BrowserCode.OnionBrowser2,
                    BrowserCode.AiBrowser,
                    BrowserCode.HaloBrowser,
                    BrowserCode.MmboxXBrowser,
                    BrowserCode.TvBrowserInternet,
                    BrowserCode.XnBrowse,
                    BrowserCode.OpenBrowserLite,
                    BrowserCode.Cromite,
                    BrowserCode.Mises,
                    BrowserCode.PuffinIncognitoBrowser,
                    BrowserCode.PuffinWebBrowser,
                    BrowserCode.PuffinSecureBrowser,
                    BrowserCode.PuffinCloudBrowser,
                    BrowserCode.CloakPrivateBrowser,
                    BrowserCode.AlohaBrowser,
                    BrowserCode.Pluma,
                    BrowserCode.WhaleTvBrowser,
                    BrowserCode.Singlebox,
                    BrowserCode.HerondBrowser,
                    BrowserCode.QuarkPc,
                    BrowserCode.Quetta,
                    BrowserCode.HeyTapBrowser,
                }.ToFrozenSet()
            },
            {
                BrowserNames.Firefox,
                new[]
                {
                    BrowserCode.Firefox,
                    BrowserCode.Basilisk,
                    BrowserCode.Byffox,
                    BrowserCode.BlackHawk,
                    BrowserCode.BorealisNavigator,
                    BrowserCode.Centaury,
                    BrowserCode.Cunaguaro,
                    BrowserCode.Epic,
                    BrowserCode.FirefoxMobileIos,
                    BrowserCode.Firebird,
                    BrowserCode.Fennec,
                    BrowserCode.ArcticFox,
                    BrowserCode.FirefoxMobile,
                    BrowserCode.FirefoxRocket,
                    BrowserCode.FirefoxReality,
                    BrowserCode.IceCat,
                    BrowserCode.Lolifox,
                    BrowserCode.Prism,
                    BrowserCode.Iceweasel,
                    BrowserCode.Light,
                    BrowserCode.PolyBrowser,
                    BrowserCode.MicroB,
                    BrowserCode.Minimo,
                    BrowserCode.Mobicip,
                    BrowserCode.Mypal,
                    BrowserCode.Orca,
                    BrowserCode.Ordissimo,
                    BrowserCode.PrivacyWall,
                    BrowserCode.Phoenix,
                    BrowserCode.Qazweb,
                    BrowserCode.SafeExamBrowser,
                    BrowserCode.Swiftfox,
                    BrowserCode.TenFourFox,
                    BrowserCode.TOnlineDeBrowser,
                    BrowserCode.Waterfox,
                    BrowserCode.Zvu,
                    BrowserCode.Floorp,
                    BrowserCode.AolShield,
                    BrowserCode.ImperviousBrowser,
                    BrowserCode.PirateBrowser,
                    BrowserCode.KNinja,
                    BrowserCode.Wyzo,
                    BrowserCode.Vonkeror,
                    BrowserCode.WebianShell,
                    BrowserCode.Classilla,
                    BrowserCode.NaenaraBrowser,
                    BrowserCode.Swiftweasel,
                }.ToFrozenSet()
            },
            {
                BrowserNames.InternetExplorer,
                new[]
                {
                    BrowserCode.InternetExplorer,
                    BrowserCode.CrazyBrowser,
                    BrowserCode.Browzar,
                    BrowserCode.IeMobile,
                    BrowserCode.MicrosoftEdge,
                    BrowserCode.AolExplorer,
                    BrowserCode.AcooBrowser,
                    BrowserCode.GreenBrowser,
                    BrowserCode.PocketInternetExplorer,
                }.ToFrozenSet()
            },
            { BrowserNames.Konqueror, new[] { BrowserCode.Konqueror }.ToFrozenSet() },
            { BrowserNames.NetFront, new[] { BrowserCode.NetFront }.ToFrozenSet() },
            { BrowserNames.NetSurf, new[] { BrowserCode.NetSurf }.ToFrozenSet() },
            {
                BrowserNames.NokiaBrowser,
                new[]
                {
                    BrowserCode.NokiaBrowser,
                    BrowserCode.Dorado,
                    BrowserCode.NokiaOssBrowser,
                    BrowserCode.NokiaOviBrowser,
                }.ToFrozenSet()
            },
            {
                BrowserNames.Opera,
                new[]
                {
                    BrowserCode.Opera,
                    BrowserCode.OperaNeon,
                    BrowserCode.OperaDevices,
                    BrowserCode.OperaMini,
                    BrowserCode.OperaMobile,
                    BrowserCode.OperaNext,
                    BrowserCode.OperaTouch,
                    BrowserCode.OperaMiniIos,
                    BrowserCode.OperaGx,
                    BrowserCode.OperaCrypto,
                }.ToFrozenSet()
            },
            {
                BrowserNames.Safari,
                new[]
                {
                    BrowserCode.Safari,
                    BrowserCode.SpBrowser,
                    BrowserCode.MobileSafari,
                    BrowserCode.SogouMobileBrowser,
                    BrowserCode.SafariTechnologyPreview,
                }.ToFrozenSet()
            },
            { BrowserNames.SailfishBrowser, new[] { BrowserCode.SailfishBrowser }.ToFrozenSet() },
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

        MobileOnlyBrowsers = new[]
        {
            BrowserCode.PhoneBrowser360,
            BrowserCode.AlohaBrowserLite,
            BrowserCode.Arvin,
            BrowserCode.BLine,
            BrowserCode.Coast,
            BrowserCode.CoolBrowser,
            BrowserCode.CosBrowser,
            BrowserCode.Cornowser,
            BrowserCode.DBrowser,
            BrowserCode.Mises,
            BrowserCode.DeltaBrowser,
            BrowserCode.EuiBrowser,
            BrowserCode.EzBrowser,
            BrowserCode.FirefoxFocus,
            BrowserCode.FirefoxMobile,
            BrowserCode.FirefoxRocket,
            BrowserCode.FauxBrowser,
            BrowserCode.GhosteryPrivacyBrowser,
            BrowserCode.GinxDroidBrowser,
            BrowserCode.GoBrowser,
            BrowserCode.HawkTurboBrowser,
            BrowserCode.HuaweiBrowserMobile,
            BrowserCode.Isivioo,
            BrowserCode.JapanBrowser,
            BrowserCode.KodeBrowser,
            BrowserCode.MCent,
            BrowserCode.MobileSafari,
            BrowserCode.Minimo,
            BrowserCode.MeizuBrowser,
            BrowserCode.NoxBrowser,
            BrowserCode.OculusBrowser,
            BrowserCode.OperaMini,
            BrowserCode.OperaMobile,
            BrowserCode.Smooz,
            BrowserCode.PuffinWebBrowser,
            BrowserCode.PrivacyWall,
            BrowserCode.PerfectBrowser,
            BrowserCode.Quark,
            BrowserCode.RealmeBrowser,
            BrowserCode.StartInternetBrowser,
            BrowserCode.SpBrowser,
            BrowserCode.SailfishBrowser,
            BrowserCode.SamsungBrowser,
            BrowserCode.Stargon,
            BrowserCode.Skyfire,
            BrowserCode.Streamy,
            BrowserCode.SuperFastBrowser,
            BrowserCode.StampyBrowser,
            BrowserCode.UcBrowserHd,
            BrowserCode.UcBrowserMini,
            BrowserCode.UcBrowserTurbo,
            BrowserCode.VenusBrowser,
            BrowserCode.VivoBrowser,
            BrowserCode.WearInternetBrowser,
            BrowserCode.WebExplorer,
            BrowserCode.YaaniBrowser,
            BrowserCode.IBrowser,
            BrowserCode.IBrowserMini,
            BrowserCode.HawkQuickBrowser,
            BrowserCode.ReqwirelessWebViewer,
            BrowserCode.HiBrowser,
            BrowserCode.ApnBrowser,
            BrowserCode.AdBlockBrowser,
            BrowserCode.YouCare,
            BrowserCode.PocketBookBrowser,
            BrowserCode.MonumentBrowser,
            BrowserCode.ApusBrowser,
            BrowserCode.AskCom,
            BrowserCode.UiBrowserMini,
            BrowserCode.SavySoda,
            BrowserCode.SavannahBrowser,
            BrowserCode.SurfBrowser,
            BrowserCode.SotiSurf,
            BrowserCode.LexiBrowser,
            BrowserCode.SmartBrowser,
            BrowserCode.BelvaBrowser,
            BrowserCode.Lilo,
            BrowserCode.FloatBrowser,
            BrowserCode.KidsSafeBrowser,
            BrowserCode.VBrowser,
            BrowserCode.CgBrowser,
            BrowserCode.AzkaBrowser,
            BrowserCode.MmxBrowser,
            BrowserCode.BitchuteBrowser,
            BrowserCode.NovaVideoDownloaderPro,
            BrowserCode.PronHubBrowser,
            BrowserCode.FrostPlus,
            BrowserCode.DucBrowser,
            BrowserCode.DesiBrowser,
            BrowserCode.PhantomMe,
            BrowserCode.OpenBrowser,
            BrowserCode.XoolooInternet,
            BrowserCode.UBrowser,
            BrowserCode.Bloket,
            BrowserCode.VastBrowser,
            BrowserCode.XVpn,
            BrowserCode.Amerigo,
            BrowserCode.XBrowserProSuperFast,
            BrowserCode.PrivacyBrowser18Plus,
            BrowserCode.BeyondPrivateBrowser,
            BrowserCode.BlackLionBrowser,
            BrowserCode.TucMiniBrowser,
            BrowserCode.AppBrowzer,
            BrowserCode.SxBrowser,
            BrowserCode.FieryBrowser,
            BrowserCode.Yagi,
            BrowserCode.NextWordBrowser,
            BrowserCode.NakedBrowserPro,
            BrowserCode.Browser1Dm,
            BrowserCode.Browser1DmPlus,
            BrowserCode.AdultBrowser,
            BrowserCode.XnxBrowser,
            BrowserCode.XtremeCast,
            BrowserCode.XBrowserLite,
            BrowserCode.SweetBrowser,
            BrowserCode.HtcBrowser,
            BrowserCode.Browlser,
            BrowserCode.BanglaBrowser,
            BrowserCode.SoundyBrowser,
            BrowserCode.IvviBrowser,
            BrowserCode.OddBrowser,
            BrowserCode.Pawxy,
            BrowserCode.OrNetBrowser,
            BrowserCode.BrowsBit,
            BrowserCode.Alva,
            BrowserCode.PicoBrowser,
            BrowserCode.WorldBrowser,
            BrowserCode.EveryBrowser,
            BrowserCode.InBrowser,
            BrowserCode.InstaBrowser,
            BrowserCode.VertexSurf,
            BrowserCode.HollaWebBrowser,
            BrowserCode.MarsLabWebBrowser,
            BrowserCode.SunflowerBrowser,
            BrowserCode.CaveBrowser,
            BrowserCode.ZordoBrowser,
            BrowserCode.DarkBrowser,
            BrowserCode.FreedomBrowser,
            BrowserCode.PrivateInternetBrowser,
            BrowserCode.Frost,
            BrowserCode.AirfindSecureBrowser,
            BrowserCode.SecureX,
            BrowserCode.Nuviu,
            BrowserCode.FGet,
            BrowserCode.Thor,
            BrowserCode.IncognitoBrowser,
            BrowserCode.GodzillaBrowser,
            BrowserCode.OceanBrowser,
            BrowserCode.Qmamu,
            BrowserCode.BfBrowser,
            BrowserCode.BroKeepBrowser,
            BrowserCode.OnionBrowser,
            BrowserCode.ProxyBrowser,
            BrowserCode.HotBrowser,
            BrowserCode.VdBrowser,
            BrowserCode.GoBrowser2,
            BrowserCode.Bang,
            BrowserCode.OnBrowserLite,
            BrowserCode.DiigoBrowser,
            BrowserCode.TrueLocationBrowser,
            BrowserCode.MixerBoxAi,
            BrowserCode.YouBrowser,
            BrowserCode.MaxBrowser,
            BrowserCode.LeganBrowser,
            BrowserCode.OjrBrowser,
            BrowserCode.InvoltaGo,
            BrowserCode.HabitBrowser,
            BrowserCode.OwlBrowser,
            BrowserCode.Orbitum,
            BrowserCode.Photon,
            BrowserCode.KeyboardBrowser,
            BrowserCode.StealthBrowser,
            BrowserCode.TalkTo,
            BrowserCode.Proxynet,
            BrowserCode.GoodBrowser,
            BrowserCode.Proxyium,
            BrowserCode.Vuhuv,
            BrowserCode.FireBrowser,
            BrowserCode.LightningBrowserPlus,
            BrowserCode.DarkWeb,
            BrowserCode.DarkWebPrivate,
            BrowserCode.SkyLeap,
            BrowserCode.Kitt,
            BrowserCode.NookBrowser,
            BrowserCode.Kun,
            BrowserCode.WukongBrowser,
            BrowserCode.MotorolaInternetBrowser,
            BrowserCode.UPhoneBrowser,
            BrowserCode.ZteBrowser,
            BrowserCode.Presearch,
            BrowserCode.Ninesky,
            BrowserCode.Veera,
            BrowserCode.PintarBrowser,
            BrowserCode.BrowserMini,
            BrowserCode.FossBrowser,
            BrowserCode.PeachBrowser,
            BrowserCode.AppTecSecureBrowser,
            BrowserCode.ProxyFox,
            BrowserCode.ProxyMax,
            BrowserCode.KeepSolidBrowser,
            BrowserCode.OnionBrowser2,
            BrowserCode.AiBrowser,
            BrowserCode.HaloBrowser,
            BrowserCode.MmboxXBrowser,
            BrowserCode.XnBrowse,
            BrowserCode.OpenBrowserLite,
            BrowserCode.PuffinIncognitoBrowser,
            BrowserCode.PuffinCloudBrowser,
            BrowserCode.CloakPrivateBrowser,
            BrowserCode.Pluma,
            BrowserCode.PocketInternetExplorer,
        }.ToFrozenSet();

        ClientHintBrandMappings = new Dictionary<string, FrozenSet<string>>
        {
            { BrowserNames.Chrome, new[] { "Google Chrome" }.ToFrozenSet() },
            { BrowserNames.ChromeWebview, new[] { "Android WebView" }.ToFrozenSet() },
            { BrowserNames.DuckDuckGoPrivacyBrowser, new[] { "DuckDuckGo" }.ToFrozenSet() },
            { BrowserNames.EdgeWebView, new[] { "Microsoft Edge WebView2" }.ToFrozenSet() },
            { BrowserNames.MiBrowser, new[] { "Miui Browser", "XiaoMiBrowser" }.ToFrozenSet() },
            { BrowserNames.MicrosoftEdge, new[] { "Edge" }.ToFrozenSet() },
            { BrowserNames.NortonPrivateBrowser, new[] { "Norton Secure Browser" }.ToFrozenSet() },
            { BrowserNames.VewdBrowser, new[] { "Vewd Core" }.ToFrozenSet() },
        }.ToFrozenDictionary();

        PriorityBrowsers = new[]
        {
            BrowserCode.Atom,
            BrowserCode.AlohaBrowser,
            BrowserCode.HuaweiBrowser,
            BrowserCode.OjrBrowser,
            BrowserCode.MiBrowser,
            BrowserCode.OperaMobile,
            BrowserCode.Opera,
            BrowserCode.Veera,
        }.ToFrozenSet();

        ChromiumBrowsers = new[]
        {
            BrowserCode.Chromium,
            BrowserCode.ChromeWebview,
            BrowserCode.AndroidBrowser,
        }.ToFrozenSet();

        ChromeSafariRegex = new Regex(
            @"Chrome/.+ Safari/537\.36",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );
        CypressOrPhantomJsRegex = new Regex("Cypress|PhantomJS", RegexOptions.Compiled);
        IridiumVersionRegex = new Regex("^202[0-4]", RegexOptions.Compiled);
    }

    public BrowserParser(UaDetectorOptions? uaDetectorOptions = null)
    {
        _uaDetectorOptions = uaDetectorOptions ?? new UaDetectorOptions();
        _cache = uaDetectorOptions?.Cache;
        _clientParser = new ClientParser();
        _botParser = new BotParser(new BotParserOptions { Cache = _cache });
    }

    private static string ApplyClientHintBrandMapping(string brand)
    {
        foreach (var clientHint in ClientHintBrandMappings)
        {
            if (clientHint.Value.Contains(brand))
            {
                return clientHint.Key;
            }
        }

        return brand;
    }

    private static bool TryMapNameToFamily(string name, [NotNullWhen((true))] out string? result)
    {
        if (BrowserRegistry.BrowserNameMappings.TryGetValue(name, out var code))
        {
            return TryMapCodeToFamily(code, out result);
        }

        result = null;
        return false;
    }

    private static bool TryMapCodeToFamily(
        BrowserCode code,
        [NotNullWhen((true))] out string? result
    )
    {
        foreach (var browserFamily in BrowserFamilyMappings)
        {
            if (browserFamily.Value.Contains(code))
            {
                result = browserFamily.Key;
                return true;
            }
        }

        result = null;
        return false;
    }

    private static string? BuildEngine(
        string userAgent,
        BrowserEngine? engine,
        string? browserVersion
    )
    {
        var result = engine?.Default;

        if (engine?.Versions?.Count > 0 && browserVersion?.Length > 0)
        {
            foreach (var version in engine.Versions)
            {
                if (
                    ParserExtensions.TryCompareVersions(
                        browserVersion,
                        version.Key,
                        out var comparisonResult
                    )
                    && comparisonResult < 0
                )
                {
                    continue;
                }

                result = version.Value;
            }
        }

        if (result is null or { Length: 0 })
        {
            EngineParser.TryParse(userAgent, out result);
        }

        return result;
    }

    private string? BuildEngineVersion(string userAgent, string? engine)
    {
        if (engine is null or { Length: 0 })
        {
            return null;
        }

        EngineVersionParser.TryParse(userAgent, engine, out var result);
        return ParserExtensions.BuildVersion(result, _uaDetectorOptions.VersionTruncation);
    }

    private static bool TryGetBrowserCode(
        string browserName,
        [NotNullWhen(true)] out BrowserCode? result
    )
    {
        browserName = browserName.CollapseSpaces();
        var hasBrowserSuffix = browserName.EndsWith("Browser");

        if (
            BrowserRegistry.BrowserNameMappings.TryGetValue(browserName, out var browserCode)
            || (
                hasBrowserSuffix
                // TODO: Remove this once net462 support is dropped
                && BrowserRegistry.BrowserNameMappings.TryGetValue(
                    browserName.Substring(0, browserName.Length - 7).TrimEnd(),
                    out browserCode
                )
            /*&& BrowserRegistry.BrowserNameMappings.TryGetValue(
                browserName[..^7].TrimEnd(),
                out browserCode
            )*/
            )
            || (
                !hasBrowserSuffix
                && BrowserRegistry.BrowserNameMappings.TryGetValue(
                    $"{browserName} Browser",
                    out browserCode
                )
            )
            || (
                CompactToFullNameMappings.TryGetValue(browserName.RemoveSpaces(), out var fullName)
                && BrowserRegistry.BrowserNameMappings.TryGetValue(fullName, out browserCode)
            )
        )
        {
            result = browserCode;
        }
        else
        {
            result = null;
        }

        return result is not null;
    }

    private bool TryParseBrowserFromClientHints(
        ClientHints clientHints,
        [NotNullWhen(true)] out ClientHintsBrowserInfo? result
    )
    {
        if (clientHints.FullVersionList.Count == 0)
        {
            result = null;
            return false;
        }

        string? name = null,
            version = null;
        BrowserCode? code = null;

        foreach (var fullVersion in clientHints.FullVersionList)
        {
            var browserName = ApplyClientHintBrandMapping(fullVersion.Key);

            if (TryGetBrowserCode(browserName, out var browserCode))
            {
                name = BrowserRegistry.BrowserCodeMappings[browserCode.Value];
                code = browserCode;
                version = fullVersion.Value;
            }

            // Exit if the detected browser brand is not Chromium or Microsoft Edge, otherwise, continue searching.
            if (
                name?.Length > 0
                && name != BrowserNames.Chromium
                && name != BrowserNames.MicrosoftEdge
            )
            {
                break;
            }
        }

        if (name is null or { Length: 0 } || code is null)
        {
            result = null;
            return false;
        }

        if (clientHints.UaFullVersion?.Length > 0)
        {
            version = clientHints.UaFullVersion;
        }

        result = new ClientHintsBrowserInfo
        {
            Name = name,
            Code = code.Value,
            Version = ParserExtensions.BuildVersion(version, _uaDetectorOptions.VersionTruncation),
        };

        return true;
    }

    private bool TryParseBrowserFromUserAgent(
        string userAgent,
        [NotNullWhen(true)] out UserAgentBrowserInfo? result
    )
    {
        Match? match = null;
        Browser? browser = null;

        foreach (var browserEntry in Browsers)
        {
            match = browserEntry.Regex.Match(userAgent);

            if (match.Success)
            {
                browser = browserEntry;
                break;
            }
        }

        if (browser is null || match is null || !match.Success)
        {
            result = null;
            return false;
        }

        string name = ParserExtensions.FormatWithMatch(browser.Name, match);

        if (BrowserRegistry.BrowserNameMappings.TryGetValue(name, out var code))
        {
            var version = ParserExtensions.BuildVersion(
                browser.Version,
                match,
                _uaDetectorOptions.VersionTruncation
            );
            var engine = BuildEngine(userAgent, browser.Engine, version);
            var engineVersion = BuildEngineVersion(userAgent, engine);

            result = new UserAgentBrowserInfo
            {
                Name = name,
                Code = code,
                Version = version,
                Engine = engine,
                EngineVersion = engineVersion,
            };
        }
        else
        {
            result = null;
        }

        return result is not null;
    }

    /// <summary>
    /// Checks if the first version is a truncated form of the second version and still considered equal.
    /// For example, version "1.2" would be considered equal to "1.2.0" or "1.2.0.0".
    /// </summary>
    private static bool IsSameTruncatedVersion(string shortVersion, string fullVersion)
    {
        return shortVersion.Length < fullVersion.Length
            && ParserExtensions.TryCompareVersions(
                shortVersion,
                fullVersion,
                out var comparisonResult
            )
            && comparisonResult == 0;
    }

    public bool TryParse(string userAgent, [NotNullWhen(true)] out BrowserInfo? result)
    {
        return TryParse(userAgent, ImmutableDictionary<string, string?>.Empty, out result);
    }

    public bool TryParse(
        string userAgent,
        IDictionary<string, string?> headers,
        [NotNullWhen(true)] out BrowserInfo? result
    )
    {
        if (!_uaDetectorOptions.DisableBotDetection && _botParser.IsBot(userAgent))
        {
            result = null;
            return false;
        }

        var clientHints = ClientHints.Create(headers);

        if (_clientParser.IsClient(userAgent, clientHints))
        {
            result = null;
            return false;
        }

        if (ParserExtensions.TryRestoreUserAgent(userAgent, clientHints, out var restoredUserAgent))
        {
            userAgent = restoredUserAgent;
        }

        var cacheKey = $"{CacheKeyPrefix}:{userAgent}";

        if (_cache is not null && _cache.TryGet(cacheKey, out result))
        {
            return result is not null;
        }

        TryParse(userAgent, clientHints, out result);
        _cache?.Set(cacheKey, result);
        return result is not null;
    }

    internal bool TryParse(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out BrowserInfo? result
    )
    {
        string? name = null;
        BrowserCode? code = null;
        string? version = null;
        string? engine = null;
        string? engineVersion = null;

        TryParseBrowserFromUserAgent(userAgent, out var browserFromUserAgent);

        if (
            TryParseBrowserFromClientHints(clientHints, out var browserFromClientHints)
            && browserFromClientHints.Version?.Length > 0
        )
        {
            name = browserFromClientHints.Name;
            code = browserFromClientHints.Code;
            version = browserFromClientHints.Version;

            if (IridiumVersionRegex.IsMatch(version))
            {
                name = BrowserNames.Iridium;
                code = BrowserCode.Iridium;
            }

            if (browserFromUserAgent is not null)
            {
                if (
                    browserFromUserAgent.Version?.Length > 0
                    && version.StartsWith("15")
                    && browserFromUserAgent.Version.StartsWith("114")
                )
                {
                    name = BrowserNames.SecureBrowser360;
                    code = BrowserCode.SecureBrowser360;
                    engine = browserFromUserAgent.Engine;
                    engineVersion = browserFromUserAgent.EngineVersion;
                }

                if (
                    browserFromUserAgent.Version?.Length > 0
                    && (
                        PriorityBrowsers.Contains(code.Value)
                        || IsSameTruncatedVersion(version, browserFromUserAgent.Version)
                    )
                )
                {
                    version = browserFromUserAgent.Version;
                }

                if (name == BrowserNames.VewdBrowser)
                {
                    engine = browserFromUserAgent.Engine;
                    engineVersion = browserFromUserAgent.EngineVersion;
                }

                if (
                    name is BrowserNames.Chromium or BrowserNames.ChromeWebview
                    && !ChromiumBrowsers.Contains(browserFromUserAgent.Code)
                )
                {
                    name = browserFromUserAgent.Name;
                    code = browserFromUserAgent.Code;
                    version = browserFromUserAgent.Version;
                }

                // Use browser name from user agent if it already contains the "Mobile" suffix
                if ($"{name} Mobile" == browserFromUserAgent.Name)
                {
                    name = browserFromUserAgent.Name;
                    code = browserFromUserAgent.Code;
                }

                if (
                    name != browserFromUserAgent.Name
                    && TryMapNameToFamily(name, out var familyFromName)
                    && TryMapNameToFamily(browserFromUserAgent.Name, out var familyFromUserAgent)
                    && familyFromName == familyFromUserAgent
                )
                {
                    engine = browserFromUserAgent.Engine;
                    engineVersion = browserFromUserAgent.EngineVersion;
                }

                if (name == browserFromUserAgent.Name)
                {
                    engine = browserFromUserAgent.Engine;
                    engineVersion = browserFromUserAgent.EngineVersion;
                }

                if (
                    browserFromUserAgent.Version?.Length > 0
                    && version?.Length > 0
                    && browserFromUserAgent.Version.StartsWith(version)
                    && ParserExtensions.TryCompareVersions(
                        version,
                        browserFromUserAgent.Version,
                        out var comparisonResult
                    )
                    && comparisonResult < 0
                )
                {
                    version = browserFromUserAgent.Version;
                }

                if (name == BrowserNames.DuckDuckGoPrivacyBrowser)
                {
                    version = null;
                }

                if (
                    engine == BrowserEngines.Blink
                    && name != BrowserNames.Iridium
                    && engineVersion?.Length > 0
                    && ParserExtensions.TryCompareVersions(
                        engineVersion,
                        browserFromClientHints.Version,
                        out comparisonResult
                    )
                    && comparisonResult < 0
                )
                {
                    engineVersion = browserFromClientHints.Version;
                }
            }
        }
        else if (browserFromUserAgent is not null)
        {
            name = browserFromUserAgent.Name;
            code = browserFromUserAgent.Code;
            version = browserFromUserAgent.Version;
            engine = browserFromUserAgent.Engine;
            engineVersion = browserFromUserAgent.EngineVersion;
        }

        string? family = null;

        if (code is not null)
        {
            TryMapCodeToFamily(code.Value, out family);
        }

        if (
            BrowserHintParser.TryParseBrowserName(clientHints, out var browserName)
            && name != browserName
        )
        {
            version = null;

            if (BrowserRegistry.BrowserNameMappings.TryGetValue(browserName, out var browserCode))
            {
                name = browserName;
                code = browserCode;
            }

            if (ChromeSafariRegex.IsMatch(userAgent))
            {
                engine = BrowserEngines.Blink;
                engineVersion = BuildEngineVersion(userAgent, engine);

                if (code is not null)
                {
                    family = TryMapCodeToFamily(code.Value, out family)
                        ? family
                        : BrowserFamilies.Chrome;
                }
            }
        }

        if (
            name is null or { Length: 0 }
            || CypressOrPhantomJsRegex.IsMatch(userAgent)
            || code is null
        )
        {
            result = null;
            return false;
        }

        // Ignore "Blink" engine version for "Flow Browser".
        if (name == BrowserNames.FlowBrowser && engine == BrowserEngines.Blink)
        {
            engineVersion = null;
        }
        // "Every Browser" mimics a Chrome user agent on Android.
        // "TV-Browser Internet" mimics a Firefox user agent.
        else if (
            name == BrowserNames.EveryBrowser
            || (name == BrowserNames.TvBrowserInternet && engine == BrowserEngines.Gecko)
        )
        {
            family = BrowserFamilies.Chrome;
            engine = BrowserEngines.Blink;
            engineVersion = null;
        }
        else if (
            name is BrowserNames.YaaniBrowser or BrowserNames.Wolvic
            && engine == BrowserEngines.Blink
        )
        {
            family = BrowserFamilies.Chrome;
        }
        else if (
            name is BrowserNames.YaaniBrowser or BrowserNames.Wolvic
            && engine == BrowserEngines.Gecko
        )
        {
            family = BrowserFamilies.Firefox;
        }

        result = new BrowserInfo
        {
            Name = name,
            Code = code.Value,
            Family = family,
            Version = version,
            Engine =
                engine is null or { Length: 0 } && engineVersion is null or { Length: 0 }
                    ? null
                    : new EngineInfo { Name = engine, Version = engineVersion },
        };

        return true;
    }

    private sealed class ClientHintsBrowserInfo
    {
        public required string Name { get; init; }
        public required BrowserCode Code { get; init; }
        public required string? Version { get; init; }
    }

    private sealed class UserAgentBrowserInfo
    {
        public required string Name { get; init; }
        public required BrowserCode Code { get; init; }
        public required string? Version { get; init; }
        public required string? Engine { get; init; }
        public required string? EngineVersion { get; init; }
    }
}
