using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UaDetector.Models.Constants;
using UaDetector.Models.Enums;
using UaDetector.Parsers.Browsers;
using UaDetector.Regexes.Models.Browsers;
using UaDetector.Results;
using UaDetector.Utils;

namespace UaDetector.Parsers;

public sealed class BrowserParser : IBrowserParser
{
    private const string ResourceName = "Regexes.Resources.Browsers.browsers.json";
    private readonly ParserOptions _parserOptions;
    private readonly ClientParser _clientParser;
    internal static readonly IEnumerable<Browser> Browsers = RegexLoader.LoadRegexes<Browser>(ResourceName);

    internal static readonly FrozenDictionary<BrowserCode, string> BrowserCodeMapping =
        new Dictionary<BrowserCode, string>
        {
            { BrowserCode.Via, BrowserNames.Via },
            { BrowserCode.PureMiniBrowser, BrowserNames.PureMiniBrowser },
            { BrowserCode.PureLiteBrowser, BrowserNames.PureLiteBrowser },
            { BrowserCode.RaiseFastBrowser, BrowserNames.RaiseFastBrowser },
            { BrowserCode.RabbitPrivateBrowser, BrowserNames.RabbitPrivateBrowser },
            { BrowserCode.FastBrowserUcLite, BrowserNames.FastBrowserUcLite },
            { BrowserCode.FastExplorer, BrowserNames.FastExplorer },
            { BrowserCode.LightningBrowser, BrowserNames.LightningBrowser },
            { BrowserCode.CakeBrowser, BrowserNames.CakeBrowser },
            { BrowserCode.IeBrowserFast, BrowserNames.IeBrowserFast },
            { BrowserCode.VegasBrowser, BrowserNames.VegasBrowser },
            { BrowserCode.OhBrowser, BrowserNames.OhBrowser },
            { BrowserCode.OhPrivateBrowser, BrowserNames.OhPrivateBrowser },
            { BrowserCode.XBrowserMini, BrowserNames.XBrowserMini },
            { BrowserCode.SharkeeBrowser, BrowserNames.SharkeeBrowser },
            { BrowserCode.LarkBrowser, BrowserNames.LarkBrowser },
            { BrowserCode.Pluma, BrowserNames.Pluma },
            { BrowserCode.AnkaBrowser, BrowserNames.AnkaBrowser },
            { BrowserCode.AzkaBrowser, BrowserNames.AzkaBrowser },
            { BrowserCode.DragonBrowser, BrowserNames.DragonBrowser },
            { BrowserCode.EasyBrowser, BrowserNames.EasyBrowser },
            { BrowserCode.DarkWebBrowser, BrowserNames.DarkWebBrowser },
            { BrowserCode.DarkBrowser, BrowserNames.DarkBrowser },
            { BrowserCode.PrivacyBrowser18Plus, BrowserNames.PrivacyBrowser18Plus },
            { BrowserCode.Browser115, BrowserNames.Browser115 },
            { BrowserCode.Browser1Dm, BrowserNames.Browser1Dm },
            { BrowserCode.Browser1DmPlus, BrowserNames.Browser1DmPlus },
            { BrowserCode.Browser2345, BrowserNames.Browser2345 },
            { BrowserCode.SecureBrowser360, BrowserNames.SecureBrowser360 },
            { BrowserCode.PhoneBrowser360, BrowserNames.PhoneBrowser360 },
            { BrowserCode.Browser7654, BrowserNames.Browser7654 },
            { BrowserCode.AvantBrowser, BrowserNames.AvantBrowser },
            { BrowserCode.ABrowse, BrowserNames.ABrowse },
            { BrowserCode.AcooBrowser, BrowserNames.AcooBrowser },
            { BrowserCode.AdBlockBrowser, BrowserNames.AdBlockBrowser },
            { BrowserCode.AdultBrowser, BrowserNames.AdultBrowser },
            { BrowserCode.AiBrowser, BrowserNames.AiBrowser },
            { BrowserCode.AirfindSecureBrowser, BrowserNames.AirfindSecureBrowser },
            { BrowserCode.AntFresco, BrowserNames.AntFresco },
            { BrowserCode.AntGalio, BrowserNames.AntGalio },
            { BrowserCode.AlohaBrowser, BrowserNames.AlohaBrowser },
            { BrowserCode.AlohaBrowserLite, BrowserNames.AlohaBrowserLite },
            { BrowserCode.Alva, BrowserNames.Alva },
            { BrowserCode.AltiBrowser, BrowserNames.AltiBrowser },
            { BrowserCode.Amaya, BrowserNames.Amaya },
            { BrowserCode.AmazeBrowser, BrowserNames.AmazeBrowser },
            { BrowserCode.Amerigo, BrowserNames.Amerigo },
            { BrowserCode.Amigo, BrowserNames.Amigo },
            { BrowserCode.AndroidBrowser, BrowserNames.AndroidBrowser },
            { BrowserCode.AolExplorer, BrowserNames.AolExplorer },
            { BrowserCode.AolDesktop, BrowserNames.AolDesktop },
            { BrowserCode.AolShield, BrowserNames.AolShield },
            { BrowserCode.AolShieldPro, BrowserNames.AolShieldPro },
            { BrowserCode.Aplix, BrowserNames.Aplix },
            { BrowserCode.AppBrowzer, BrowserNames.AppBrowzer },
            { BrowserCode.AppTecSecureBrowser, BrowserNames.AppTecSecureBrowser },
            { BrowserCode.ApusBrowser, BrowserNames.ApusBrowser },
            { BrowserCode.Arora, BrowserNames.Arora },
            { BrowserCode.ArcticFox, BrowserNames.ArcticFox },
            { BrowserCode.AmigaVoyager, BrowserNames.AmigaVoyager },
            { BrowserCode.AmigaAweb, BrowserNames.AmigaAweb },
            { BrowserCode.ApnBrowser, BrowserNames.ApnBrowser },
            { BrowserCode.Arachne, BrowserNames.Arachne },
            { BrowserCode.ArcSearch, BrowserNames.ArcSearch },
            { BrowserCode.ArmorflyBrowser, BrowserNames.ArmorflyBrowser },
            { BrowserCode.Arvin, BrowserNames.Arvin },
            { BrowserCode.AskCom, BrowserNames.AskCom },
            { BrowserCode.AsusBrowser, BrowserNames.AsusBrowser },
            { BrowserCode.Atom, BrowserNames.Atom },
            { BrowserCode.AtomicWebBrowser, BrowserNames.AtomicWebBrowser },
            { BrowserCode.Atlas, BrowserNames.Atlas },
            { BrowserCode.AvastSecureBrowser, BrowserNames.AvastSecureBrowser },
            { BrowserCode.AvgSecureBrowser, BrowserNames.AvgSecureBrowser },
            { BrowserCode.AviraSecureBrowser, BrowserNames.AviraSecureBrowser },
            { BrowserCode.AwoX, BrowserNames.AwoX },
            { BrowserCode.Awesomium, BrowserNames.Awesomium },
            { BrowserCode.BasicWebBrowser, BrowserNames.BasicWebBrowser },
            { BrowserCode.BeakerBrowser, BrowserNames.BeakerBrowser },
            { BrowserCode.Beamrise, BrowserNames.Beamrise },
            { BrowserCode.BfBrowser, BrowserNames.BfBrowser },
            { BrowserCode.BlackBerryBrowser, BrowserNames.BlackBerryBrowser },
            { BrowserCode.Bluefy, BrowserNames.Bluefy },
            { BrowserCode.BrowseHere, BrowserNames.BrowseHere },
            { BrowserCode.BrowserHupPro, BrowserNames.BrowserHupPro },
            { BrowserCode.BaiduBrowser, BrowserNames.BaiduBrowser },
            { BrowserCode.BaiduSpark, BrowserNames.BaiduSpark },
            { BrowserCode.Bang, BrowserNames.Bang },
            { BrowserCode.BanglaBrowser, BrowserNames.BanglaBrowser },
            { BrowserCode.Basilisk, BrowserNames.Basilisk },
            { BrowserCode.BelvaBrowser, BrowserNames.BelvaBrowser },
            { BrowserCode.BeyondPrivateBrowser, BrowserNames.BeyondPrivateBrowser },
            { BrowserCode.Beonex, BrowserNames.Beonex },
            { BrowserCode.BerryBrowser, BrowserNames.BerryBrowser },
            { BrowserCode.BitchuteBrowser, BrowserNames.BitchuteBrowser },
            { BrowserCode.BizBrowser, BrowserNames.BizBrowser },
            { BrowserCode.BlackHawk, BrowserNames.BlackHawk },
            { BrowserCode.Bloket, BrowserNames.Bloket },
            { BrowserCode.Bunjalloo, BrowserNames.Bunjalloo },
            { BrowserCode.BLine, BrowserNames.BLine },
            { BrowserCode.BlackLionBrowser, BrowserNames.BlackLionBrowser },
            { BrowserCode.BlueBrowser, BrowserNames.BlueBrowser },
            { BrowserCode.Bonsai, BrowserNames.Bonsai },
            { BrowserCode.BorealisNavigator, BrowserNames.BorealisNavigator },
            { BrowserCode.Brave, BrowserNames.Brave },
            { BrowserCode.BriskBard, BrowserNames.BriskBard },
            { BrowserCode.BroKeepBrowser, BrowserNames.BroKeepBrowser },
            { BrowserCode.BrowspeedBrowser, BrowserNames.BrowspeedBrowser },
            { BrowserCode.BrowseX, BrowserNames.BrowseX },
            { BrowserCode.Browzar, BrowserNames.Browzar },
            { BrowserCode.Browlser, BrowserNames.Browlser },
            { BrowserCode.BrowserMini, BrowserNames.BrowserMini },
            { BrowserCode.BrowsBit, BrowserNames.BrowsBit },
            { BrowserCode.Biyubi, BrowserNames.Biyubi },
            { BrowserCode.Byffox, BrowserNames.Byffox },
            { BrowserCode.BxeBrowser, BrowserNames.BxeBrowser },
            { BrowserCode.Camino, BrowserNames.Camino },
            { BrowserCode.Catalyst, BrowserNames.Catalyst },
            { BrowserCode.Catsxp, BrowserNames.Catsxp },
            { BrowserCode.CaveBrowser, BrowserNames.CaveBrowser },
            { BrowserCode.CCleaner, BrowserNames.CCleaner },
            { BrowserCode.CgBrowser, BrowserNames.CgBrowser },
            { BrowserCode.ChanjetCloud, BrowserNames.ChanjetCloud },
            { BrowserCode.Chedot, BrowserNames.Chedot },
            { BrowserCode.CherryBrowser, BrowserNames.CherryBrowser },
            { BrowserCode.Centaury, BrowserNames.Centaury },
            { BrowserCode.Cliqz, BrowserNames.Cliqz },
            { BrowserCode.CocCoc, BrowserNames.CocCoc },
            { BrowserCode.CoolBrowser, BrowserNames.CoolBrowser },
            { BrowserCode.Colibri, BrowserNames.Colibri },
            { BrowserCode.ColumbusBrowser, BrowserNames.ColumbusBrowser },
            { BrowserCode.ComodoDragon, BrowserNames.ComodoDragon },
            { BrowserCode.Coast, BrowserNames.Coast },
            { BrowserCode.Charon, BrowserNames.Charon },
            { BrowserCode.CmBrowser, BrowserNames.CmBrowser },
            { BrowserCode.CmMini, BrowserNames.CmMini },
            { BrowserCode.ChromeFrame, BrowserNames.ChromeFrame },
            { BrowserCode.HeadlessChrome, BrowserNames.HeadlessChrome },
            { BrowserCode.Chrome, BrowserNames.Chrome },
            { BrowserCode.ChromeMobileIos, BrowserNames.ChromeMobileIos },
            { BrowserCode.Conkeror, BrowserNames.Conkeror },
            { BrowserCode.ChromeMobile, BrowserNames.ChromeMobile },
            { BrowserCode.Chowbo, BrowserNames.Chowbo },
            { BrowserCode.Classilla, BrowserNames.Classilla },
            { BrowserCode.CoolNovo, BrowserNames.CoolNovo },
            { BrowserCode.ColomBrowser, BrowserNames.ColomBrowser },
            { BrowserCode.CometBird, BrowserNames.CometBird },
            { BrowserCode.ComfortBrowser, BrowserNames.ComfortBrowser },
            { BrowserCode.CosBrowser, BrowserNames.CosBrowser },
            { BrowserCode.Cornowser, BrowserNames.Cornowser },
            { BrowserCode.ChimLac, BrowserNames.ChimLac },
            { BrowserCode.ChromePlus, BrowserNames.ChromePlus },
            { BrowserCode.Chromium, BrowserNames.Chromium },
            { BrowserCode.ChromiumGost, BrowserNames.ChromiumGost },
            { BrowserCode.Cyberfox, BrowserNames.Cyberfox },
            { BrowserCode.Cheshire, BrowserNames.Cheshire },
            { BrowserCode.Cromite, BrowserNames.Cromite },
            { BrowserCode.CrowBrowser, BrowserNames.CrowBrowser },
            { BrowserCode.Crusta, BrowserNames.Crusta },
            { BrowserCode.CravingExplorer, BrowserNames.CravingExplorer },
            { BrowserCode.CrazyBrowser, BrowserNames.CrazyBrowser },
            { BrowserCode.Cunaguaro, BrowserNames.Cunaguaro },
            { BrowserCode.ChromeWebview, BrowserNames.ChromeWebview },
            { BrowserCode.CyBrowser, BrowserNames.CyBrowser },
            { BrowserCode.DBrowser, BrowserNames.DBrowser },
            { BrowserCode.PeepsDBrowser, BrowserNames.PeepsDBrowser },
            { BrowserCode.DarkWeb, BrowserNames.DarkWeb },
            { BrowserCode.DarkWebPrivate, BrowserNames.DarkWebPrivate },
            { BrowserCode.DebuggableBrowser, BrowserNames.DebuggableBrowser },
            { BrowserCode.Decentr, BrowserNames.Decentr },
            { BrowserCode.DeepnetExplorer, BrowserNames.DeepnetExplorer },
            { BrowserCode.DegDegan, BrowserNames.DegDegan },
            { BrowserCode.Deledao, BrowserNames.Deledao },
            { BrowserCode.DeltaBrowser, BrowserNames.DeltaBrowser },
            { BrowserCode.DesiBrowser, BrowserNames.DesiBrowser },
            { BrowserCode.DeskBrowse, BrowserNames.DeskBrowse },
            { BrowserCode.Dezor, BrowserNames.Dezor },
            { BrowserCode.DiigoBrowser, BrowserNames.DiigoBrowser },
            { BrowserCode.DoCoMo, BrowserNames.DoCoMo },
            { BrowserCode.Dolphin, BrowserNames.Dolphin },
            { BrowserCode.DolphinZero, BrowserNames.DolphinZero },
            { BrowserCode.Dorado, BrowserNames.Dorado },
            { BrowserCode.DotBrowser, BrowserNames.DotBrowser },
            { BrowserCode.Dooble, BrowserNames.Dooble },
            { BrowserCode.Dillo, BrowserNames.Dillo },
            { BrowserCode.DucBrowser, BrowserNames.DucBrowser },
            { BrowserCode.DuckDuckGoPrivacyBrowser, BrowserNames.DuckDuckGoPrivacyBrowser },
            { BrowserCode.EastBrowser, BrowserNames.EastBrowser },
            { BrowserCode.Ecosia, BrowserNames.Ecosia },
            { BrowserCode.EdgeWebView, BrowserNames.EdgeWebView },
            { BrowserCode.EveryBrowser, BrowserNames.EveryBrowser },
            { BrowserCode.Epic, BrowserNames.Epic },
            { BrowserCode.Elinks, BrowserNames.Elinks },
            { BrowserCode.EinkBro, BrowserNames.EinkBro },
            { BrowserCode.ElementBrowser, BrowserNames.ElementBrowser },
            { BrowserCode.ElementsBrowser, BrowserNames.ElementsBrowser },
            { BrowserCode.Eolie, BrowserNames.Eolie },
            { BrowserCode.ExploreBrowser, BrowserNames.ExploreBrowser },
            { BrowserCode.EzBrowser, BrowserNames.EzBrowser },
            { BrowserCode.EudoraWeb, BrowserNames.EudoraWeb },
            { BrowserCode.EuiBrowser, BrowserNames.EuiBrowser },
            { BrowserCode.GnomeWeb, BrowserNames.GnomeWeb },
            { BrowserCode.GBrowser, BrowserNames.GBrowser },
            { BrowserCode.EspialTvBrowser, BrowserNames.EspialTvBrowser },
            { BrowserCode.FGet, BrowserNames.FGet },
            { BrowserCode.Falkon, BrowserNames.Falkon },
            { BrowserCode.FauxBrowser, BrowserNames.FauxBrowser },
            { BrowserCode.FireBrowser, BrowserNames.FireBrowser },
            { BrowserCode.FieryBrowser, BrowserNames.FieryBrowser },
            { BrowserCode.FirefoxMobileIos, BrowserNames.FirefoxMobileIos },
            { BrowserCode.Firebird, BrowserNames.Firebird },
            { BrowserCode.Fluid, BrowserNames.Fluid },
            { BrowserCode.Fennec, BrowserNames.Fennec },
            { BrowserCode.Firefox, BrowserNames.Firefox },
            { BrowserCode.FirefoxFocus, BrowserNames.FirefoxFocus },
            { BrowserCode.FirefoxReality, BrowserNames.FirefoxReality },
            { BrowserCode.FirefoxRocket, BrowserNames.FirefoxRocket },
            { BrowserCode.FirefoxKlar, BrowserNames.FirefoxKlar },
            { BrowserCode.FloatBrowser, BrowserNames.FloatBrowser },
            { BrowserCode.Flock, BrowserNames.Flock },
            { BrowserCode.Floorp, BrowserNames.Floorp },
            { BrowserCode.Flow, BrowserNames.Flow },
            { BrowserCode.FlowBrowser, BrowserNames.FlowBrowser },
            { BrowserCode.FirefoxMobile, BrowserNames.FirefoxMobile },
            { BrowserCode.Fireweb, BrowserNames.Fireweb },
            { BrowserCode.FirewebNavigator, BrowserNames.FirewebNavigator },
            { BrowserCode.FlashBrowser, BrowserNames.FlashBrowser },
            { BrowserCode.Flast, BrowserNames.Flast },
            { BrowserCode.Flyperlink, BrowserNames.Flyperlink },
            { BrowserCode.FossBrowser, BrowserNames.FossBrowser },
            { BrowserCode.FreeU, BrowserNames.FreeU },
            { BrowserCode.FreedomBrowser, BrowserNames.FreedomBrowser },
            { BrowserCode.Frost, BrowserNames.Frost },
            { BrowserCode.FrostPlus, BrowserNames.FrostPlus },
            { BrowserCode.Fulldive, BrowserNames.Fulldive },
            { BrowserCode.Galeon, BrowserNames.Galeon },
            { BrowserCode.Gener8, BrowserNames.Gener8 },
            { BrowserCode.GhosteryPrivacyBrowser, BrowserNames.GhosteryPrivacyBrowser },
            { BrowserCode.GinxDroidBrowser, BrowserNames.GinxDroidBrowser },
            { BrowserCode.GlassBrowser, BrowserNames.GlassBrowser },
            { BrowserCode.GodzillaBrowser, BrowserNames.GodzillaBrowser },
            { BrowserCode.GoodBrowser, BrowserNames.GoodBrowser },
            { BrowserCode.GoogleEarth, BrowserNames.GoogleEarth },
            { BrowserCode.GoogleEarthPro, BrowserNames.GoogleEarthPro },
            { BrowserCode.GogGalaxy, BrowserNames.GogGalaxy },
            { BrowserCode.GoBrowser, BrowserNames.GoBrowser },
            { BrowserCode.GoKu, BrowserNames.GoKu },
            { BrowserCode.GoBrowser2, BrowserNames.GoBrowser2 },
            { BrowserCode.GreenBrowser, BrowserNames.GreenBrowser },
            { BrowserCode.HabitBrowser, BrowserNames.HabitBrowser },
            { BrowserCode.HaloBrowser, BrowserNames.HaloBrowser },
            { BrowserCode.HarmanBrowser, BrowserNames.HarmanBrowser },
            { BrowserCode.HasBrowser, BrowserNames.HasBrowser },
            { BrowserCode.HawkTurboBrowser, BrowserNames.HawkTurboBrowser },
            { BrowserCode.HawkQuickBrowser, BrowserNames.HawkQuickBrowser },
            { BrowserCode.Helio, BrowserNames.Helio },
            { BrowserCode.HerondBrowser, BrowserNames.HerondBrowser },
            { BrowserCode.HexaWebBrowser, BrowserNames.HexaWebBrowser },
            { BrowserCode.HiBrowser, BrowserNames.HiBrowser },
            { BrowserCode.HolaBrowser, BrowserNames.HolaBrowser },
            { BrowserCode.HollaWebBrowser, BrowserNames.HollaWebBrowser },
            { BrowserCode.HotBrowser, BrowserNames.HotBrowser },
            { BrowserCode.HotJava, BrowserNames.HotJava },
            { BrowserCode.HonorBrowser, BrowserNames.HonorBrowser },
            { BrowserCode.HtcBrowser, BrowserNames.HtcBrowser },
            { BrowserCode.HuaweiBrowserMobile, BrowserNames.HuaweiBrowserMobile },
            { BrowserCode.HuaweiBrowser, BrowserNames.HuaweiBrowser },
            { BrowserCode.HubBrowser, BrowserNames.HubBrowser },
            { BrowserCode.IBrowser, BrowserNames.IBrowser },
            { BrowserCode.IBrowserMini, BrowserNames.IBrowserMini },
            { BrowserCode.IBrowse, BrowserNames.IBrowse },
            { BrowserCode.IDesktopPcBrowser, BrowserNames.IDesktopPcBrowser },
            { BrowserCode.ICab, BrowserNames.ICab },
            { BrowserCode.ICabMobile, BrowserNames.ICabMobile },
            { BrowserCode.INetBrowser, BrowserNames.INetBrowser },
            { BrowserCode.Iridium, BrowserNames.Iridium },
            { BrowserCode.IronMobile, BrowserNames.IronMobile },
            { BrowserCode.IceCat, BrowserNames.IceCat },
            { BrowserCode.IceDragon, BrowserNames.IceDragon },
            { BrowserCode.Isivioo, BrowserNames.Isivioo },
            { BrowserCode.IvviBrowser, BrowserNames.IvviBrowser },
            { BrowserCode.Iceweasel, BrowserNames.Iceweasel },
            { BrowserCode.ImperviousBrowser, BrowserNames.ImperviousBrowser },
            { BrowserCode.IncognitoBrowser, BrowserNames.IncognitoBrowser },
            { BrowserCode.InspectBrowser, BrowserNames.InspectBrowser },
            { BrowserCode.InstaBrowser, BrowserNames.InstaBrowser },
            { BrowserCode.InternetExplorer, BrowserNames.InternetExplorer },
            { BrowserCode.InternetBrowserSecure, BrowserNames.InternetBrowserSecure },
            { BrowserCode.InternetWebbrowser, BrowserNames.InternetWebbrowser },
            { BrowserCode.IntuneManagedBrowser, BrowserNames.IntuneManagedBrowser },
            { BrowserCode.IndianUcMiniBrowser, BrowserNames.IndianUcMiniBrowser },
            { BrowserCode.InBrowser, BrowserNames.InBrowser },
            { BrowserCode.InvoltaGo, BrowserNames.InvoltaGo },
            { BrowserCode.IeMobile, BrowserNames.IeMobile },
            { BrowserCode.Iron, BrowserNames.Iron },
            { BrowserCode.JapanBrowser, BrowserNames.JapanBrowser },
            { BrowserCode.Jasmine, BrowserNames.Jasmine },
            { BrowserCode.JavaFx, BrowserNames.JavaFx },
            { BrowserCode.Jelly, BrowserNames.Jelly },
            { BrowserCode.JigBrowser, BrowserNames.JigBrowser },
            { BrowserCode.JigBrowserPlus, BrowserNames.JigBrowserPlus },
            { BrowserCode.JioSphere, BrowserNames.JioSphere },
            { BrowserCode.JuziBrowser, BrowserNames.JuziBrowser },
            { BrowserCode.KBrowser, BrowserNames.KBrowser },
            { BrowserCode.KeepsafeBrowser, BrowserNames.KeepsafeBrowser },
            { BrowserCode.KeepSolidBrowser, BrowserNames.KeepSolidBrowser },
            { BrowserCode.KidsSafeBrowser, BrowserNames.KidsSafeBrowser },
            { BrowserCode.KindleBrowser, BrowserNames.KindleBrowser },
            { BrowserCode.KMeleon, BrowserNames.KMeleon },
            { BrowserCode.KNinja, BrowserNames.KNinja },
            { BrowserCode.Konqueror, BrowserNames.Konqueror },
            { BrowserCode.Kapiko, BrowserNames.Kapiko },
            { BrowserCode.KeyboardBrowser, BrowserNames.KeyboardBrowser },
            { BrowserCode.Kinza, BrowserNames.Kinza },
            { BrowserCode.Kitt, BrowserNames.Kitt },
            { BrowserCode.Kiwi, BrowserNames.Kiwi },
            { BrowserCode.KodeBrowser, BrowserNames.KodeBrowser },
            { BrowserCode.Kun, BrowserNames.Kun },
            { BrowserCode.KutoMiniBrowser, BrowserNames.KutoMiniBrowser },
            { BrowserCode.Kylo, BrowserNames.Kylo },
            { BrowserCode.Kazehakase, BrowserNames.Kazehakase },
            { BrowserCode.CheetahBrowser, BrowserNames.CheetahBrowser },
            { BrowserCode.Ladybird, BrowserNames.Ladybird },
            { BrowserCode.LagatosBrowser, BrowserNames.LagatosBrowser },
            { BrowserCode.LeganBrowser, BrowserNames.LeganBrowser },
            { BrowserCode.LexiBrowser, BrowserNames.LexiBrowser },
            { BrowserCode.LenovoBrowser, BrowserNames.LenovoBrowser },
            { BrowserCode.LieBaoFast, BrowserNames.LieBaoFast },
            { BrowserCode.LgBrowser, BrowserNames.LgBrowser },
            { BrowserCode.Light, BrowserNames.Light },
            { BrowserCode.LightningBrowserPlus, BrowserNames.LightningBrowserPlus },
            { BrowserCode.Lilo, BrowserNames.Lilo },
            { BrowserCode.Links, BrowserNames.Links },
            { BrowserCode.LiriBrowser, BrowserNames.LiriBrowser },
            { BrowserCode.LogicUiTvBrowser, BrowserNames.LogicUiTvBrowser },
            { BrowserCode.Lolifox, BrowserNames.Lolifox },
            { BrowserCode.Lotus, BrowserNames.Lotus },
            { BrowserCode.LovenseBrowser, BrowserNames.LovenseBrowser },
            { BrowserCode.LtBrowser, BrowserNames.LtBrowser },
            { BrowserCode.LuaKit, BrowserNames.LuaKit },
            { BrowserCode.LujoTvBrowser, BrowserNames.LujoTvBrowser },
            { BrowserCode.Lulumi, BrowserNames.Lulumi },
            { BrowserCode.Lunascape, BrowserNames.Lunascape },
            { BrowserCode.LunascapeLite, BrowserNames.LunascapeLite },
            { BrowserCode.Lynx, BrowserNames.Lynx },
            { BrowserCode.LynketBrowser, BrowserNames.LynketBrowser },
            { BrowserCode.Mandarin, BrowserNames.Mandarin },
            { BrowserCode.Maple, BrowserNames.Maple },
            { BrowserCode.MarsLabWebBrowser, BrowserNames.MarsLabWebBrowser },
            { BrowserCode.MaxBrowser, BrowserNames.MaxBrowser },
            { BrowserCode.MCent, BrowserNames.MCent },
            { BrowserCode.MicroB, BrowserNames.MicroB },
            { BrowserCode.NcsaMosaic, BrowserNames.NcsaMosaic },
            { BrowserCode.MeizuBrowser, BrowserNames.MeizuBrowser },
            { BrowserCode.Mercury, BrowserNames.Mercury },
            { BrowserCode.MeBrowser, BrowserNames.MeBrowser },
            { BrowserCode.MobileSafari, BrowserNames.MobileSafari },
            { BrowserCode.Midori, BrowserNames.Midori },
            { BrowserCode.MidoriLite, BrowserNames.MidoriLite },
            { BrowserCode.MixerBoxAi, BrowserNames.MixerBoxAi },
            { BrowserCode.Mobicip, BrowserNames.Mobicip },
            { BrowserCode.MiBrowser, BrowserNames.MiBrowser },
            { BrowserCode.MobileSilk, BrowserNames.MobileSilk },
            { BrowserCode.MogokBrowser, BrowserNames.MogokBrowser },
            { BrowserCode.MotorolaInternetBrowser, BrowserNames.MotorolaInternetBrowser },
            { BrowserCode.Minimo, BrowserNames.Minimo },
            { BrowserCode.MintBrowser, BrowserNames.MintBrowser },
            { BrowserCode.Maxthon, BrowserNames.Maxthon },
            { BrowserCode.MaxTubeBrowser, BrowserNames.MaxTubeBrowser },
            { BrowserCode.Maelstrom, BrowserNames.Maelstrom },
            { BrowserCode.Mises, BrowserNames.Mises },
            { BrowserCode.MmxBrowser, BrowserNames.MmxBrowser },
            { BrowserCode.MxNitro, BrowserNames.MxNitro },
            { BrowserCode.Mypal, BrowserNames.Mypal },
            { BrowserCode.MonumentBrowser, BrowserNames.MonumentBrowser },
            { BrowserCode.MauiWapBrowser, BrowserNames.MauiWapBrowser },
            { BrowserCode.NaenaraBrowser, BrowserNames.NaenaraBrowser },
            { BrowserCode.NavigateurWeb, BrowserNames.NavigateurWeb },
            { BrowserCode.NakedBrowser, BrowserNames.NakedBrowser },
            { BrowserCode.NakedBrowserPro, BrowserNames.NakedBrowserPro },
            { BrowserCode.NfsBrowser, BrowserNames.NfsBrowser },
            { BrowserCode.Ninetails, BrowserNames.Ninetails },
            { BrowserCode.NokiaBrowser, BrowserNames.NokiaBrowser },
            { BrowserCode.NokiaOssBrowser, BrowserNames.NokiaOssBrowser },
            { BrowserCode.NokiaOviBrowser, BrowserNames.NokiaOviBrowser },
            { BrowserCode.NortonPrivateBrowser, BrowserNames.NortonPrivateBrowser },
            { BrowserCode.NoxBrowser, BrowserNames.NoxBrowser },
            { BrowserCode.NomOneVrBrowser, BrowserNames.NomOneVrBrowser },
            { BrowserCode.NookBrowser, BrowserNames.NookBrowser },
            { BrowserCode.NetSurf, BrowserNames.NetSurf },
            { BrowserCode.NetFront, BrowserNames.NetFront },
            { BrowserCode.NetFrontLife, BrowserNames.NetFrontLife },
            { BrowserCode.NetPositive, BrowserNames.NetPositive },
            { BrowserCode.Netscape, BrowserNames.Netscape },
            { BrowserCode.NextWordBrowser, BrowserNames.NextWordBrowser },
            { BrowserCode.Ninesky, BrowserNames.Ninesky },
            { BrowserCode.NtentBrowser, BrowserNames.NtentBrowser },
            { BrowserCode.NuantiMeta, BrowserNames.NuantiMeta },
            { BrowserCode.Nuviu, BrowserNames.Nuviu },
            { BrowserCode.OceanBrowser, BrowserNames.OceanBrowser },
            { BrowserCode.OculusBrowser, BrowserNames.OculusBrowser },
            { BrowserCode.OddBrowser, BrowserNames.OddBrowser },
            { BrowserCode.OperaMiniIos, BrowserNames.OperaMiniIos },
            { BrowserCode.Obigo, BrowserNames.Obigo },
            { BrowserCode.Odin, BrowserNames.Odin },
            { BrowserCode.OdinBrowser, BrowserNames.OdinBrowser },
            { BrowserCode.OceanHero, BrowserNames.OceanHero },
            { BrowserCode.OdysseyWebBrowser, BrowserNames.OdysseyWebBrowser },
            { BrowserCode.OffByOne, BrowserNames.OffByOne },
            { BrowserCode.OfficeBrowser, BrowserNames.OfficeBrowser },
            { BrowserCode.OhHaiBrowser, BrowserNames.OhHaiBrowser },
            { BrowserCode.OnBrowserLite, BrowserNames.OnBrowserLite },
            { BrowserCode.OneBrowser, BrowserNames.OneBrowser },
            { BrowserCode.OnionBrowser, BrowserNames.OnionBrowser },
            { BrowserCode.OnionBrowser2, BrowserNames.OnionBrowser2 },
            { BrowserCode.OperaCrypto, BrowserNames.OperaCrypto },
            { BrowserCode.OperaGx, BrowserNames.OperaGx },
            { BrowserCode.OperaNeon, BrowserNames.OperaNeon },
            { BrowserCode.OperaDevices, BrowserNames.OperaDevices },
            { BrowserCode.OperaMini, BrowserNames.OperaMini },
            { BrowserCode.OperaMobile, BrowserNames.OperaMobile },
            { BrowserCode.Opera, BrowserNames.Opera },
            { BrowserCode.OperaNext, BrowserNames.OperaNext },
            { BrowserCode.OperaTouch, BrowserNames.OperaTouch },
            { BrowserCode.Orbitum, BrowserNames.Orbitum },
            { BrowserCode.Orca, BrowserNames.Orca },
            { BrowserCode.Ordissimo, BrowserNames.Ordissimo },
            { BrowserCode.Oregano, BrowserNames.Oregano },
            { BrowserCode.OriginInGameOverlay, BrowserNames.OriginInGameOverlay },
            { BrowserCode.OrigynWebBrowser, BrowserNames.OrigynWebBrowser },
            { BrowserCode.OrNetBrowser, BrowserNames.OrNetBrowser },
            { BrowserCode.OpenwaveMobileBrowser, BrowserNames.OpenwaveMobileBrowser },
            { BrowserCode.OpenFin, BrowserNames.OpenFin },
            { BrowserCode.OpenBrowser, BrowserNames.OpenBrowser },
            { BrowserCode.OpenBrowser4U, BrowserNames.OpenBrowser4U },
            { BrowserCode.OpenBrowserFast5G, BrowserNames.OpenBrowserFast5G },
            { BrowserCode.OpenBrowserLite, BrowserNames.OpenBrowserLite },
            { BrowserCode.OpenTvBrowser, BrowserNames.OpenTvBrowser },
            { BrowserCode.OmniWeb, BrowserNames.OmniWeb },
            { BrowserCode.OtterBrowser, BrowserNames.OtterBrowser },
            { BrowserCode.OwlBrowser, BrowserNames.OwlBrowser },
            { BrowserCode.OjrBrowser, BrowserNames.OjrBrowser },
            { BrowserCode.PalmBlazer, BrowserNames.PalmBlazer },
            { BrowserCode.PocketInternetExplorer, BrowserNames.PocketInternetExplorer },
            { BrowserCode.PaleMoon, BrowserNames.PaleMoon },
            { BrowserCode.Polypane, BrowserNames.Polypane },
            { BrowserCode.Prism, BrowserNames.Prism },
            { BrowserCode.OppoBrowser, BrowserNames.OppoBrowser },
            { BrowserCode.OpusBrowser, BrowserNames.OpusBrowser },
            { BrowserCode.PalmPre, BrowserNames.PalmPre },
            { BrowserCode.PuffinCloudBrowser, BrowserNames.PuffinCloudBrowser },
            { BrowserCode.PuffinIncognitoBrowser, BrowserNames.PuffinIncognitoBrowser },
            { BrowserCode.PuffinSecureBrowser, BrowserNames.PuffinSecureBrowser },
            { BrowserCode.PuffinWebBrowser, BrowserNames.PuffinWebBrowser },
            { BrowserCode.PalmWebPro, BrowserNames.PalmWebPro },
            { BrowserCode.Palmscape, BrowserNames.Palmscape },
            { BrowserCode.Pawxy, BrowserNames.Pawxy },
            { BrowserCode.PeachBrowser, BrowserNames.PeachBrowser },
            { BrowserCode.PerfectBrowser, BrowserNames.PerfectBrowser },
            { BrowserCode.Perk, BrowserNames.Perk },
            { BrowserCode.PhantomMe, BrowserNames.PhantomMe },
            { BrowserCode.PhantomBrowser, BrowserNames.PhantomBrowser },
            { BrowserCode.Phoenix, BrowserNames.Phoenix },
            { BrowserCode.PhoenixBrowser, BrowserNames.PhoenixBrowser },
            { BrowserCode.Photon, BrowserNames.Photon },
            { BrowserCode.PintarBrowser, BrowserNames.PintarBrowser },
            { BrowserCode.PirateBrowser, BrowserNames.PirateBrowser },
            { BrowserCode.PicoBrowser, BrowserNames.PicoBrowser },
            { BrowserCode.PlayFreeBrowser, BrowserNames.PlayFreeBrowser },
            { BrowserCode.PocketBookBrowser, BrowserNames.PocketBookBrowser },
            { BrowserCode.Polaris, BrowserNames.Polaris },
            { BrowserCode.Polarity, BrowserNames.Polarity },
            { BrowserCode.PolyBrowser, BrowserNames.PolyBrowser },
            { BrowserCode.Presearch, BrowserNames.Presearch },
            { BrowserCode.PrivacyBrowser, BrowserNames.PrivacyBrowser },
            { BrowserCode.PrivacyWall, BrowserNames.PrivacyWall },
            { BrowserCode.PrivacyExplorerFastSafe, BrowserNames.PrivacyExplorerFastSafe },
            { BrowserCode.PrivacyPioneerBrowser, BrowserNames.PrivacyPioneerBrowser },
            { BrowserCode.PrivateInternetBrowser, BrowserNames.PrivateInternetBrowser },
            { BrowserCode.ProxyBrowser, BrowserNames.ProxyBrowser },
            { BrowserCode.Proxyium, BrowserNames.Proxyium },
            { BrowserCode.Proxynet, BrowserNames.Proxynet },
            { BrowserCode.ProxyFox, BrowserNames.ProxyFox },
            { BrowserCode.ProxyMax, BrowserNames.ProxyMax },
            { BrowserCode.PiBrowser, BrowserNames.PiBrowser },
            { BrowserCode.PronHubBrowser, BrowserNames.PronHubBrowser },
            { BrowserCode.PsiSecureBrowser, BrowserNames.PsiSecureBrowser },
            { BrowserCode.ReqwirelessWebViewer, BrowserNames.ReqwirelessWebViewer },
            { BrowserCode.Roccat, BrowserNames.Roccat },
            { BrowserCode.MicrosoftEdge, BrowserNames.MicrosoftEdge },
            { BrowserCode.Qazweb, BrowserNames.Qazweb },
            { BrowserCode.Qiyu, BrowserNames.Qiyu },
            { BrowserCode.QjyTvBrowser, BrowserNames.QjyTvBrowser },
            { BrowserCode.Qmamu, BrowserNames.Qmamu },
            { BrowserCode.QuickSearchTv, BrowserNames.QuickSearchTv },
            { BrowserCode.QqBrowserLite, BrowserNames.QqBrowserLite },
            { BrowserCode.QqBrowserMini, BrowserNames.QqBrowserMini },
            { BrowserCode.QqBrowser, BrowserNames.QqBrowser },
            { BrowserCode.QuickBrowser, BrowserNames.QuickBrowser },
            { BrowserCode.Qutebrowser, BrowserNames.Qutebrowser },
            { BrowserCode.Quark, BrowserNames.Quark },
            { BrowserCode.QupZilla, BrowserNames.QupZilla },
            { BrowserCode.QwantMobile, BrowserNames.QwantMobile },
            { BrowserCode.QtWeb, BrowserNames.QtWeb },
            { BrowserCode.QtWebEngine, BrowserNames.QtWebEngine },
            { BrowserCode.RakutenBrowser, BrowserNames.RakutenBrowser },
            { BrowserCode.RakutenWebSearch, BrowserNames.RakutenWebSearch },
            { BrowserCode.RaspbianChromium, BrowserNames.RaspbianChromium },
            { BrowserCode.RcaTorExplorer, BrowserNames.RcaTorExplorer },
            { BrowserCode.RealmeBrowser, BrowserNames.RealmeBrowser },
            { BrowserCode.Rekonq, BrowserNames.Rekonq },
            { BrowserCode.RockMelt, BrowserNames.RockMelt },
            { BrowserCode.RokuBrowser, BrowserNames.RokuBrowser },
            { BrowserCode.SamsungBrowser, BrowserNames.SamsungBrowser },
            { BrowserCode.SamsungBrowserLite, BrowserNames.SamsungBrowserLite },
            { BrowserCode.SailfishBrowser, BrowserNames.SailfishBrowser },
            { BrowserCode.SberBrowser, BrowserNames.SberBrowser },
            { BrowserCode.SeewoBrowser, BrowserNames.SeewoBrowser },
            { BrowserCode.SemcBrowser, BrowserNames.SemcBrowser },
            { BrowserCode.SogouExplorer, BrowserNames.SogouExplorer },
            { BrowserCode.SogouMobileBrowser, BrowserNames.SogouMobileBrowser },
            { BrowserCode.SotiSurf, BrowserNames.SotiSurf },
            { BrowserCode.SoulBrowser, BrowserNames.SoulBrowser },
            { BrowserCode.SoundyBrowser, BrowserNames.SoundyBrowser },
            { BrowserCode.Safari, BrowserNames.Safari },
            { BrowserCode.SafariTechnologyPreview, BrowserNames.SafariTechnologyPreview },
            { BrowserCode.SafeExamBrowser, BrowserNames.SafeExamBrowser },
            { BrowserCode.SalamWeb, BrowserNames.SalamWeb },
            { BrowserCode.SavannahBrowser, BrowserNames.SavannahBrowser },
            { BrowserCode.SavySoda, BrowserNames.SavySoda },
            { BrowserCode.SecureBrowser, BrowserNames.SecureBrowser },
            { BrowserCode.SFive, BrowserNames.SFive },
            { BrowserCode.Shiira, BrowserNames.Shiira },
            { BrowserCode.Sidekick, BrowserNames.Sidekick },
            { BrowserCode.SimpleBrowser, BrowserNames.SimpleBrowser },
            { BrowserCode.SilverMobUs, BrowserNames.SilverMobUs },
            { BrowserCode.Singlebox, BrowserNames.Singlebox },
            { BrowserCode.Sizzy, BrowserNames.Sizzy },
            { BrowserCode.Skye, BrowserNames.Skye },
            { BrowserCode.Skyfire, BrowserNames.Skyfire },
            { BrowserCode.SkyLeap, BrowserNames.SkyLeap },
            { BrowserCode.SeraphicSraf, BrowserNames.SeraphicSraf },
            { BrowserCode.SiteKiosk, BrowserNames.SiteKiosk },
            { BrowserCode.Sleipnir, BrowserNames.Sleipnir },
            { BrowserCode.SlimBoat, BrowserNames.SlimBoat },
            { BrowserCode.Slimjet, BrowserNames.Slimjet },
            { BrowserCode.SpBrowser, BrowserNames.SpBrowser },
            { BrowserCode.SonySmallBrowser, BrowserNames.SonySmallBrowser },
            { BrowserCode.SecurePrivateBrowser, BrowserNames.SecurePrivateBrowser },
            { BrowserCode.SecureX, BrowserNames.SecureX },
            { BrowserCode.StampyBrowser, BrowserNames.StampyBrowser },
            { BrowserCode.SevenStar, BrowserNames.SevenStar },
            { BrowserCode.SmartBrowser, BrowserNames.SmartBrowser },
            { BrowserCode.SmartSearchAndWebBrowser, BrowserNames.SmartSearchAndWebBrowser },
            { BrowserCode.SmartLenovoBrowser, BrowserNames.SmartLenovoBrowser },
            { BrowserCode.Smooz, BrowserNames.Smooz },
            { BrowserCode.Snowshoe, BrowserNames.Snowshoe },
            { BrowserCode.Spark, BrowserNames.Spark },
            { BrowserCode.SpectreBrowser, BrowserNames.SpectreBrowser },
            { BrowserCode.Splash, BrowserNames.Splash },
            { BrowserCode.SputnikBrowser, BrowserNames.SputnikBrowser },
            { BrowserCode.Sunrise, BrowserNames.Sunrise },
            { BrowserCode.SunflowerBrowser, BrowserNames.SunflowerBrowser },
            { BrowserCode.SuperBird, BrowserNames.SuperBird },
            { BrowserCode.SuperFastBrowser, BrowserNames.SuperFastBrowser },
            { BrowserCode.SuperFastBrowser2, BrowserNames.SuperFastBrowser2 },
            { BrowserCode.SushiBrowser, BrowserNames.SushiBrowser },
            { BrowserCode.Surf, BrowserNames.Surf },
            { BrowserCode.SurfBrowser, BrowserNames.SurfBrowser },
            { BrowserCode.SurfyBrowser, BrowserNames.SurfyBrowser },
            { BrowserCode.Stargon, BrowserNames.Stargon },
            { BrowserCode.StartInternetBrowser, BrowserNames.StartInternetBrowser },
            { BrowserCode.StealthBrowser, BrowserNames.StealthBrowser },
            { BrowserCode.SteamInGameOverlay, BrowserNames.SteamInGameOverlay },
            { BrowserCode.Streamy, BrowserNames.Streamy },
            { BrowserCode.Swiftfox, BrowserNames.Swiftfox },
            { BrowserCode.Swiftweasel, BrowserNames.Swiftweasel },
            { BrowserCode.SeznamBrowser, BrowserNames.SeznamBrowser },
            { BrowserCode.SweetBrowser, BrowserNames.SweetBrowser },
            { BrowserCode.SxBrowser, BrowserNames.SxBrowser },
            { BrowserCode.TBrowser, BrowserNames.TBrowser },
            { BrowserCode.TBrowser2, BrowserNames.TBrowser2 },
            { BrowserCode.TOnlineDeBrowser, BrowserNames.TOnlineDeBrowser },
            { BrowserCode.TalkTo, BrowserNames.TalkTo },
            { BrowserCode.TaoBrowser, BrowserNames.TaoBrowser },
            { BrowserCode.Tararia, BrowserNames.Tararia },
            { BrowserCode.Thor, BrowserNames.Thor },
            { BrowserCode.TorBrowser, BrowserNames.TorBrowser },
            { BrowserCode.TenFourFox, BrowserNames.TenFourFox },
            { BrowserCode.TentaBrowser, BrowserNames.TentaBrowser },
            { BrowserCode.TeslaBrowser, BrowserNames.TeslaBrowser },
            { BrowserCode.TizenBrowser, BrowserNames.TizenBrowser },
            { BrowserCode.TintBrowser, BrowserNames.TintBrowser },
            { BrowserCode.TrueLocationBrowser, BrowserNames.TrueLocationBrowser },
            { BrowserCode.TucMiniBrowser, BrowserNames.TucMiniBrowser },
            { BrowserCode.Tusk, BrowserNames.Tusk },
            { BrowserCode.Tungsten, BrowserNames.Tungsten },
            { BrowserCode.ToGate, BrowserNames.ToGate },
            { BrowserCode.TotalBrowser, BrowserNames.TotalBrowser },
            { BrowserCode.TqBrowser, BrowserNames.TqBrowser },
            { BrowserCode.TweakStyle, BrowserNames.TweakStyle },
            { BrowserCode.TvBro, BrowserNames.TvBro },
            { BrowserCode.TvBrowserInternet, BrowserNames.TvBrowserInternet },
            { BrowserCode.UBrowser, BrowserNames.UBrowser },
            { BrowserCode.UBrowser2, BrowserNames.UBrowser2 },
            { BrowserCode.UcBrowser, BrowserNames.UcBrowser },
            { BrowserCode.UcBrowserHd, BrowserNames.UcBrowserHd },
            { BrowserCode.UcBrowserMini, BrowserNames.UcBrowserMini },
            { BrowserCode.UcBrowserTurbo, BrowserNames.UcBrowserTurbo },
            { BrowserCode.UiBrowserMini, BrowserNames.UiBrowserMini },
            { BrowserCode.UPhoneBrowser, BrowserNames.UPhoneBrowser },
            { BrowserCode.UrBrowser, BrowserNames.UrBrowser },
            { BrowserCode.Uzbl, BrowserNames.Uzbl },
            { BrowserCode.UmeBrowser, BrowserNames.UmeBrowser },
            { BrowserCode.VBrowser, BrowserNames.VBrowser },
            { BrowserCode.VastBrowser, BrowserNames.VastBrowser },
            { BrowserCode.VdBrowser, BrowserNames.VdBrowser },
            { BrowserCode.Veera, BrowserNames.Veera },
            { BrowserCode.VenusBrowser, BrowserNames.VenusBrowser },
            { BrowserCode.VewdBrowser, BrowserNames.VewdBrowser },
            { BrowserCode.VibeMate, BrowserNames.VibeMate },
            { BrowserCode.NovaVideoDownloaderPro, BrowserNames.NovaVideoDownloaderPro },
            { BrowserCode.ViasatBrowser, BrowserNames.ViasatBrowser },
            { BrowserCode.Vivaldi, BrowserNames.Vivaldi },
            { BrowserCode.VivoBrowser, BrowserNames.VivoBrowser },
            { BrowserCode.VividBrowserMini, BrowserNames.VividBrowserMini },
            { BrowserCode.VisionMobileBrowser, BrowserNames.VisionMobileBrowser },
            { BrowserCode.VertexSurf, BrowserNames.VertexSurf },
            { BrowserCode.VMwareAirWatch, BrowserNames.VMwareAirWatch },
            { BrowserCode.VmsMosaic, BrowserNames.VmsMosaic },
            { BrowserCode.Vonkeror, BrowserNames.Vonkeror },
            { BrowserCode.Vuhuv, BrowserNames.Vuhuv },
            { BrowserCode.WearInternetBrowser, BrowserNames.WearInternetBrowser },
            { BrowserCode.WebExplorer, BrowserNames.WebExplorer },
            { BrowserCode.WebBrowserAndExplorer, BrowserNames.WebBrowserAndExplorer },
            { BrowserCode.WebianShell, BrowserNames.WebianShell },
            { BrowserCode.WebDiscover, BrowserNames.WebDiscover },
            { BrowserCode.WebPositive, BrowserNames.WebPositive },
            { BrowserCode.WeltweitimnetzBrowser, BrowserNames.WeltweitimnetzBrowser },
            { BrowserCode.Wexond, BrowserNames.Wexond },
            { BrowserCode.Waterfox, BrowserNames.Waterfox },
            { BrowserCode.WaveBrowser, BrowserNames.WaveBrowser },
            { BrowserCode.Wavebox, BrowserNames.Wavebox },
            { BrowserCode.WhaleBrowser, BrowserNames.WhaleBrowser },
            { BrowserCode.WhaleTvBrowser, BrowserNames.WhaleTvBrowser },
            { BrowserCode.WOsBrowser, BrowserNames.WOsBrowser },
            { BrowserCode.W3M, BrowserNames.W3M },
            { BrowserCode.WeTabBrowser, BrowserNames.WeTabBrowser },
            { BrowserCode.WorldBrowser, BrowserNames.WorldBrowser },
            { BrowserCode.Wolvic, BrowserNames.Wolvic },
            { BrowserCode.WukongBrowser, BrowserNames.WukongBrowser },
            { BrowserCode.Wyzo, BrowserNames.Wyzo },
            { BrowserCode.Yagi, BrowserNames.Yagi },
            { BrowserCode.YahooJapanBrowser, BrowserNames.YahooJapanBrowser },
            { BrowserCode.YandexBrowser, BrowserNames.YandexBrowser },
            { BrowserCode.YandexBrowserCorp, BrowserNames.YandexBrowserCorp },
            { BrowserCode.YandexBrowserLite, BrowserNames.YandexBrowserLite },
            { BrowserCode.YaaniBrowser, BrowserNames.YaaniBrowser },
            { BrowserCode.YoBrowser, BrowserNames.YoBrowser },
            { BrowserCode.YoloBrowser, BrowserNames.YoloBrowser },
            { BrowserCode.YouCare, BrowserNames.YouCare },
            { BrowserCode.YouBrowser, BrowserNames.YouBrowser },
            { BrowserCode.YuzuBrowser, BrowserNames.YuzuBrowser },
            { BrowserCode.XBrowser, BrowserNames.XBrowser },
            { BrowserCode.MmboxXBrowser, BrowserNames.MmboxXBrowser },
            { BrowserCode.XBrowserLite, BrowserNames.XBrowserLite },
            { BrowserCode.XVpn, BrowserNames.XVpn },
            { BrowserCode.XBrowserProSuperFast, BrowserNames.XBrowserProSuperFast },
            { BrowserCode.XnxBrowser, BrowserNames.XnxBrowser },
            { BrowserCode.XtremeCast, BrowserNames.XtremeCast },
            { BrowserCode.XStand, BrowserNames.XStand },
            { BrowserCode.Xiino, BrowserNames.Xiino },
            { BrowserCode.XnBrowse, BrowserNames.XnBrowse },
            { BrowserCode.XoolooInternet, BrowserNames.XoolooInternet },
            { BrowserCode.Xvast, BrowserNames.Xvast },
            { BrowserCode.Zetakey, BrowserNames.Zetakey },
            { BrowserCode.Zvu, BrowserNames.Zvu },
            { BrowserCode.ZircoBrowser, BrowserNames.ZircoBrowser },
            { BrowserCode.ZordoBrowser, BrowserNames.ZordoBrowser },
            { BrowserCode.ZteBrowser, BrowserNames.ZteBrowser },
        }.ToFrozenDictionary();

    internal static readonly FrozenDictionary<string, BrowserCode> BrowserNameMapping = BrowserCodeMapping
        .ToDictionary(e => e.Value, e => e.Key)
        .ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    internal static readonly FrozenDictionary<string, string> CompactToFullNameMapping;

    private static readonly FrozenDictionary<string, FrozenSet<BrowserCode>> BrowserFamilyMapping =
        new Dictionary<string, FrozenSet<BrowserCode>>
        {
            { BrowserFamilies.AndroidBrowser, new[] { BrowserCode.AndroidBrowser }.ToFrozenSet() },
            { BrowserFamilies.BlackBerryBrowser, new[] { BrowserCode.BlackBerryBrowser }.ToFrozenSet() },
            {
                BrowserFamilies.Baidu,
                new[] { BrowserCode.BaiduBrowser, BrowserCode.BaiduSpark, BrowserCode.HonorBrowser }.ToFrozenSet()
            },
            { BrowserFamilies.Amiga, new[] { BrowserCode.AmigaVoyager, BrowserCode.AmigaAweb }.ToFrozenSet() },
            {
                BrowserFamilies.Chrome, new[]
                    {
                        BrowserCode.Chrome, BrowserCode.Browser2345, BrowserCode.SevenStar, BrowserCode.Atom,
                        BrowserCode.AviraSecureBrowser, BrowserCode.AolShieldPro, BrowserCode.AolDesktop,
                        BrowserCode.AlohaBrowserLite, BrowserCode.Arvin, BrowserCode.Amigo,
                        BrowserCode.AvastSecureBrowser, BrowserCode.BeakerBrowser, BrowserCode.Beamrise,
                        BrowserCode.Brave, BrowserCode.Colibri, BrowserCode.ChimLac, BrowserCode.ChromiumGost,
                        BrowserCode.CoolBrowser, BrowserCode.Chedot, BrowserCode.CocCoc, BrowserCode.ComodoDragon,
                        BrowserCode.CmBrowser, BrowserCode.ChromeFrame, BrowserCode.CravingExplorer,
                        BrowserCode.Browser115, BrowserCode.ChromeMobileIos, BrowserCode.CCleaner,
                        BrowserCode.ChromeMobile, BrowserCode.CoolNovo, BrowserCode.ChromePlus,
                        BrowserCode.Chromium, BrowserCode.ChromeWebview, BrowserCode.Cornowser, BrowserCode.Deledao,
                        BrowserCode.DuckDuckGoPrivacyBrowser, BrowserCode.DegDegan, BrowserCode.DotBrowser,
                        BrowserCode.Ecosia, BrowserCode.ElementsBrowser, BrowserCode.EuiBrowser,
                        BrowserCode.EdgeWebView, BrowserCode.Falkon, BrowserCode.Flast, BrowserCode.GlassBrowser,
                        BrowserCode.GinxDroidBrowser, BrowserCode.OceanHero, BrowserCode.HawkTurboBrowser,
                        BrowserCode.Helio, BrowserCode.OhHaiBrowser, BrowserCode.HasBrowser, BrowserCode.IronMobile,
                        BrowserCode.Iron, BrowserCode.JapanBrowser, BrowserCode.Kinza, BrowserCode.Kiwi,
                        BrowserCode.LieBaoFast, BrowserCode.Lulumi, BrowserCode.LovenseBrowser, BrowserCode.MCent,
                        BrowserCode.Maelstrom, BrowserCode.Mandarin, BrowserCode.MonumentBrowser,
                        BrowserCode.MobileSilk, BrowserCode.MintBrowser, BrowserCode.MeizuBrowser,
                        BrowserCode.MxNitro, BrowserCode.NfsBrowser, BrowserCode.OriginInGameOverlay,
                        BrowserCode.Odin, BrowserCode.OpenFin, BrowserCode.OculusBrowser,
                        BrowserCode.PhoenixBrowser, BrowserCode.Polarity, BrowserCode.Quark,
                        BrowserCode.QtWebEngine, BrowserCode.RockMelt, BrowserCode.SteamInGameOverlay,
                        BrowserCode.Slimjet, BrowserCode.SeewoBrowser, BrowserCode.SecureBrowser,
                        BrowserCode.SamsungBrowser, BrowserCode.Stargon, BrowserCode.SeraphicSraf,
                        BrowserCode.SuperFastBrowser, BrowserCode.SFive, BrowserCode.SalamWeb, BrowserCode.Sizzy,
                        BrowserCode.SeznamBrowser, BrowserCode.StampyBrowser, BrowserCode.TaoBrowser,
                        BrowserCode.TentaBrowser, BrowserCode.ToGate, BrowserCode.TBrowser2, BrowserCode.TweakStyle,
                        BrowserCode.Tungsten, BrowserCode.TvBro, BrowserCode.UBrowser2, BrowserCode.UrBrowser,
                        BrowserCode.VenusBrowser, BrowserCode.AvgSecureBrowser, BrowserCode.Vivaldi,
                        BrowserCode.VMwareAirWatch, BrowserCode.WebExplorer, BrowserCode.WhaleBrowser,
                        BrowserCode.Xvast, BrowserCode.YahooJapanBrowser, BrowserCode.YaaniBrowser,
                        BrowserCode.FlashBrowser, BrowserCode.SpectreBrowser, BrowserCode.Bonsai,
                        BrowserCode.HarmanBrowser, BrowserCode.PsiSecureBrowser, BrowserCode.LagatosBrowser,
                        BrowserCode.LtBrowser, BrowserCode.PeepsDBrowser, BrowserCode.SushiBrowser,
                        BrowserCode.HuaweiBrowserMobile, BrowserCode.HuaweiBrowser, BrowserCode.IBrowser,
                        BrowserCode.TBrowser, BrowserCode.ChanjetCloud, BrowserCode.HawkQuickBrowser,
                        BrowserCode.HiBrowser, BrowserCode.ApnBrowser, BrowserCode.AdBlockBrowser,
                        BrowserCode.YouCare, BrowserCode.Decentr, BrowserCode.Gener8, BrowserCode.DeltaBrowser,
                        BrowserCode.ApusBrowser, BrowserCode.AskCom, BrowserCode.UiBrowserMini,
                        BrowserCode.SavySoda, BrowserCode.SavannahBrowser, BrowserCode.SurfBrowser,
                        BrowserCode.SoulBrowser, BrowserCode.SotiSurf, BrowserCode.LexiBrowser,
                        BrowserCode.SmartBrowser, BrowserCode.BelvaBrowser, BrowserCode.Lilo,
                        BrowserCode.FloatBrowser, BrowserCode.KidsSafeBrowser, BrowserCode.VBrowser,
                        BrowserCode.CgBrowser, BrowserCode.AzkaBrowser, BrowserCode.MmxBrowser,
                        BrowserCode.BitchuteBrowser, BrowserCode.NovaVideoDownloaderPro, BrowserCode.PronHubBrowser,
                        BrowserCode.FrostPlus, BrowserCode.ViasatBrowser, BrowserCode.DucBrowser,
                        BrowserCode.DesiBrowser, BrowserCode.PhantomMe, BrowserCode.OpenBrowser,
                        BrowserCode.SecurePrivateBrowser, BrowserCode.HubBrowser, BrowserCode.TeslaBrowser,
                        BrowserCode.WaveBrowser, BrowserCode.Sidekick, BrowserCode.PiBrowser,
                        BrowserCode.XoolooInternet, BrowserCode.UBrowser, BrowserCode.Bloket,
                        BrowserCode.VastBrowser, BrowserCode.XVpn, BrowserCode.NoxBrowser,
                        BrowserCode.OfficeBrowser, BrowserCode.RabbitPrivateBrowser, BrowserCode.Iridium,
                        BrowserCode.HolaBrowser, BrowserCode.Amerigo, BrowserCode.XBrowserProSuperFast,
                        BrowserCode.PrivacyBrowser18Plus, BrowserCode.BeyondPrivateBrowser,
                        BrowserCode.BlackLionBrowser, BrowserCode.TucMiniBrowser, BrowserCode.AppBrowzer,
                        BrowserCode.SxBrowser, BrowserCode.FieryBrowser, BrowserCode.Yagi,
                        BrowserCode.NextWordBrowser, BrowserCode.NakedBrowserPro, BrowserCode.Browser1Dm,
                        BrowserCode.Browser1DmPlus, BrowserCode.AdultBrowser, BrowserCode.XnxBrowser,
                        BrowserCode.XtremeCast, BrowserCode.XBrowserLite, BrowserCode.SweetBrowser,
                        BrowserCode.HtcBrowser, BrowserCode.BrowserHupPro, BrowserCode.Flyperlink,
                        BrowserCode.BanglaBrowser, BrowserCode.Wavebox, BrowserCode.SoundyBrowser,
                        BrowserCode.HeadlessChrome, BrowserCode.OddBrowser, BrowserCode.Pawxy,
                        BrowserCode.LujoTvBrowser, BrowserCode.LogicUiTvBrowser, BrowserCode.OpenTvBrowser,
                        BrowserCode.NortonPrivateBrowser, BrowserCode.Alva, BrowserCode.PicoBrowser,
                        BrowserCode.RokuBrowser, BrowserCode.WorldBrowser, BrowserCode.EveryBrowser,
                        BrowserCode.InstaBrowser, BrowserCode.VertexSurf, BrowserCode.HollaWebBrowser,
                        BrowserCode.TorBrowser, BrowserCode.MarsLabWebBrowser, BrowserCode.SunflowerBrowser,
                        BrowserCode.CaveBrowser, BrowserCode.ZordoBrowser, BrowserCode.DarkBrowser,
                        BrowserCode.FreedomBrowser, BrowserCode.CrowBrowser, BrowserCode.VewdBrowser,
                        BrowserCode.PrivateInternetBrowser, BrowserCode.Frost, BrowserCode.AirfindSecureBrowser,
                        BrowserCode.SecureX, BrowserCode.IncognitoBrowser, BrowserCode.GodzillaBrowser,
                        BrowserCode.OceanBrowser, BrowserCode.Qmamu, BrowserCode.BfBrowser,
                        BrowserCode.BroKeepBrowser, BrowserCode.ProxyBrowser, BrowserCode.HotBrowser,
                        BrowserCode.VdBrowser, BrowserCode.Skye, BrowserCode.QuickSearchTv, BrowserCode.GoBrowser2,
                        BrowserCode.RaspbianChromium, BrowserCode.Wexond, BrowserCode.Catsxp,
                        BrowserCode.IntuneManagedBrowser, BrowserCode.Bang, BrowserCode.SberBrowser,
                        BrowserCode.JioSphere, BrowserCode.OnBrowserLite, BrowserCode.LeganBrowser,
                        BrowserCode.WebDiscover, BrowserCode.Qiyu, BrowserCode.EastBrowser, BrowserCode.LiriBrowser,
                        BrowserCode.SlimBoat, BrowserCode.BasicWebBrowser, BrowserCode.Kitt,
                        BrowserCode.WukongBrowser, BrowserCode.TotalBrowser, BrowserCode.Spark,
                        BrowserCode.MiBrowser, BrowserCode.Presearch, BrowserCode.Perk, BrowserCode.Veera,
                        BrowserCode.PintarBrowser, BrowserCode.BrowserMini, BrowserCode.FossBrowser,
                        BrowserCode.PeachBrowser, BrowserCode.AppTecSecureBrowser, BrowserCode.OjrBrowser,
                        BrowserCode.Dezor, BrowserCode.Tusk, BrowserCode.PrivacyBrowser, BrowserCode.ProxyFox,
                        BrowserCode.ProxyMax, BrowserCode.KeepSolidBrowser, BrowserCode.OnionBrowser2,
                        BrowserCode.AiBrowser, BrowserCode.HaloBrowser, BrowserCode.MmboxXBrowser,
                        BrowserCode.TvBrowserInternet, BrowserCode.XnBrowse, BrowserCode.OpenBrowserLite,
                        BrowserCode.Cromite, BrowserCode.Mises, BrowserCode.PuffinIncognitoBrowser,
                        BrowserCode.PuffinWebBrowser, BrowserCode.PuffinSecureBrowser,
                        BrowserCode.PuffinCloudBrowser, BrowserCode.PrivacyPioneerBrowser, BrowserCode.AlohaBrowser,
                        BrowserCode.Pluma, BrowserCode.WhaleTvBrowser, BrowserCode.Singlebox,
                        BrowserCode.HerondBrowser,
                    }
                    .ToFrozenSet()
            },
            {
                BrowserNames.Firefox,
                new[]
                {
                    BrowserCode.Firefox, BrowserCode.Basilisk, BrowserCode.Byffox, BrowserCode.BlackHawk,
                    BrowserCode.BorealisNavigator, BrowserCode.Centaury, BrowserCode.Cunaguaro, BrowserCode.Epic,
                    BrowserCode.FirefoxMobileIos, BrowserCode.Firebird, BrowserCode.Fennec, BrowserCode.ArcticFox,
                    BrowserCode.FirefoxMobile, BrowserCode.FirefoxRocket, BrowserCode.FirefoxReality,
                    BrowserCode.IceCat, BrowserCode.Lolifox, BrowserCode.Prism, BrowserCode.Iceweasel,
                    BrowserCode.Light, BrowserCode.PolyBrowser, BrowserCode.MicroB, BrowserCode.Minimo,
                    BrowserCode.Mobicip, BrowserCode.Mypal, BrowserCode.Orca, BrowserCode.Ordissimo,
                    BrowserCode.PrivacyWall, BrowserCode.Phoenix, BrowserCode.Qazweb, BrowserCode.SafeExamBrowser,
                    BrowserCode.Swiftfox, BrowserCode.TenFourFox, BrowserCode.TOnlineDeBrowser,
                    BrowserCode.Waterfox, BrowserCode.Zvu, BrowserCode.Floorp, BrowserCode.AolShield,
                    BrowserCode.ImperviousBrowser, BrowserCode.PirateBrowser, BrowserCode.KNinja, BrowserCode.Wyzo,
                    BrowserCode.Vonkeror, BrowserCode.WebianShell, BrowserCode.Classilla,
                    BrowserCode.NaenaraBrowser, BrowserCode.Swiftweasel,
                }.ToFrozenSet()
            },
            {
                BrowserNames.InternetExplorer,
                new[]
                {
                    BrowserCode.InternetExplorer, BrowserCode.CrazyBrowser, BrowserCode.Browzar,
                    BrowserCode.IeMobile, BrowserCode.MicrosoftEdge, BrowserCode.AolExplorer,
                    BrowserCode.AcooBrowser, BrowserCode.GreenBrowser, BrowserCode.PocketInternetExplorer
                }.ToFrozenSet()
            },
            { BrowserNames.Konqueror, new[] { BrowserCode.Konqueror, }.ToFrozenSet() },
            { BrowserNames.NetFront, new[] { BrowserCode.NetFront, }.ToFrozenSet() },
            { BrowserNames.NetSurf, new[] { BrowserCode.NetSurf, }.ToFrozenSet() },
            {
                BrowserNames.NokiaBrowser,
                new[]
                {
                    BrowserCode.NokiaBrowser, BrowserCode.Dorado, BrowserCode.NokiaOssBrowser,
                    BrowserCode.NokiaOviBrowser,
                }.ToFrozenSet()
            },
            {
                BrowserNames.Opera,
                new[]
                {
                    BrowserCode.Opera, BrowserCode.OperaNeon, BrowserCode.OperaDevices, BrowserCode.OperaMini,
                    BrowserCode.OperaMobile, BrowserCode.OperaNext, BrowserCode.OperaTouch,
                    BrowserCode.OperaMiniIos, BrowserCode.OperaGx, BrowserCode.OperaCrypto,
                }.ToFrozenSet()
            },
            {
                BrowserNames.Safari,
                new[]
                {
                    BrowserCode.Safari, BrowserCode.SpBrowser, BrowserCode.MobileSafari,
                    BrowserCode.SogouMobileBrowser, BrowserCode.SafariTechnologyPreview,
                }.ToFrozenSet()
            },
            { BrowserNames.SailfishBrowser, new[] { BrowserCode.SailfishBrowser, }.ToFrozenSet() },

        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    internal static readonly FrozenSet<BrowserCode> MobileOnlyBrowsers = new[]
    {
        BrowserCode.PhoneBrowser360, BrowserCode.AlohaBrowserLite, BrowserCode.Arvin, BrowserCode.BLine,
        BrowserCode.Coast, BrowserCode.CoolBrowser, BrowserCode.CosBrowser, BrowserCode.Cornowser,
        BrowserCode.DBrowser, BrowserCode.Mises, BrowserCode.DeltaBrowser, BrowserCode.EuiBrowser,
        BrowserCode.EzBrowser, BrowserCode.FirefoxFocus, BrowserCode.FirefoxMobile, BrowserCode.FirefoxRocket,
        BrowserCode.FauxBrowser, BrowserCode.GhosteryPrivacyBrowser, BrowserCode.GinxDroidBrowser,
        BrowserCode.GoBrowser, BrowserCode.HawkTurboBrowser, BrowserCode.HuaweiBrowserMobile, BrowserCode.Isivioo,
        BrowserCode.JapanBrowser, BrowserCode.KodeBrowser, BrowserCode.MCent, BrowserCode.MobileSafari,
        BrowserCode.Minimo, BrowserCode.MeizuBrowser, BrowserCode.NoxBrowser, BrowserCode.OculusBrowser,
        BrowserCode.OperaMini, BrowserCode.OperaMobile, BrowserCode.Smooz, BrowserCode.PuffinWebBrowser,
        BrowserCode.PrivacyWall, BrowserCode.PerfectBrowser, BrowserCode.Quark, BrowserCode.RealmeBrowser,
        BrowserCode.StartInternetBrowser, BrowserCode.SpBrowser, BrowserCode.SailfishBrowser,
        BrowserCode.SamsungBrowser, BrowserCode.Stargon, BrowserCode.Skyfire, BrowserCode.Streamy,
        BrowserCode.SuperFastBrowser, BrowserCode.StampyBrowser, BrowserCode.UcBrowserHd, BrowserCode.UcBrowserMini,
        BrowserCode.UcBrowserTurbo, BrowserCode.VenusBrowser, BrowserCode.VivoBrowser,
        BrowserCode.WearInternetBrowser, BrowserCode.WebExplorer, BrowserCode.YaaniBrowser, BrowserCode.IBrowser,
        BrowserCode.IBrowserMini, BrowserCode.HawkQuickBrowser, BrowserCode.ReqwirelessWebViewer,
        BrowserCode.HiBrowser, BrowserCode.ApnBrowser, BrowserCode.AdBlockBrowser, BrowserCode.YouCare,
        BrowserCode.PocketBookBrowser, BrowserCode.MonumentBrowser, BrowserCode.ApusBrowser, BrowserCode.AskCom,
        BrowserCode.UiBrowserMini, BrowserCode.SavySoda, BrowserCode.SavannahBrowser, BrowserCode.SurfBrowser,
        BrowserCode.SotiSurf, BrowserCode.LexiBrowser, BrowserCode.SmartBrowser, BrowserCode.BelvaBrowser,
        BrowserCode.Lilo, BrowserCode.FloatBrowser, BrowserCode.KidsSafeBrowser, BrowserCode.VBrowser,
        BrowserCode.CgBrowser, BrowserCode.AzkaBrowser, BrowserCode.MmxBrowser, BrowserCode.BitchuteBrowser,
        BrowserCode.NovaVideoDownloaderPro, BrowserCode.PronHubBrowser, BrowserCode.FrostPlus,
        BrowserCode.DucBrowser, BrowserCode.DesiBrowser, BrowserCode.PhantomMe, BrowserCode.OpenBrowser,
        BrowserCode.XoolooInternet, BrowserCode.UBrowser, BrowserCode.Bloket, BrowserCode.VastBrowser,
        BrowserCode.XVpn, BrowserCode.Amerigo, BrowserCode.XBrowserProSuperFast, BrowserCode.PrivacyBrowser18Plus,
        BrowserCode.BeyondPrivateBrowser, BrowserCode.BlackLionBrowser, BrowserCode.TucMiniBrowser,
        BrowserCode.AppBrowzer, BrowserCode.SxBrowser, BrowserCode.FieryBrowser, BrowserCode.Yagi,
        BrowserCode.NextWordBrowser, BrowserCode.NakedBrowserPro, BrowserCode.Browser1Dm,
        BrowserCode.Browser1DmPlus, BrowserCode.AdultBrowser, BrowserCode.XnxBrowser, BrowserCode.XtremeCast,
        BrowserCode.XBrowserLite, BrowserCode.SweetBrowser, BrowserCode.HtcBrowser, BrowserCode.Browlser,
        BrowserCode.BanglaBrowser, BrowserCode.SoundyBrowser, BrowserCode.IvviBrowser, BrowserCode.OddBrowser,
        BrowserCode.Pawxy, BrowserCode.OrNetBrowser, BrowserCode.BrowsBit, BrowserCode.Alva,
        BrowserCode.PicoBrowser, BrowserCode.WorldBrowser, BrowserCode.EveryBrowser, BrowserCode.InBrowser,
        BrowserCode.InstaBrowser, BrowserCode.VertexSurf, BrowserCode.HollaWebBrowser,
        BrowserCode.MarsLabWebBrowser, BrowserCode.SunflowerBrowser, BrowserCode.CaveBrowser,
        BrowserCode.ZordoBrowser, BrowserCode.DarkBrowser, BrowserCode.FreedomBrowser,
        BrowserCode.PrivateInternetBrowser, BrowserCode.Frost, BrowserCode.AirfindSecureBrowser,
        BrowserCode.SecureX, BrowserCode.Nuviu, BrowserCode.FGet, BrowserCode.Thor, BrowserCode.IncognitoBrowser,
        BrowserCode.GodzillaBrowser, BrowserCode.OceanBrowser, BrowserCode.Qmamu, BrowserCode.BfBrowser,
        BrowserCode.BroKeepBrowser, BrowserCode.OnionBrowser, BrowserCode.ProxyBrowser, BrowserCode.HotBrowser,
        BrowserCode.VdBrowser, BrowserCode.GoBrowser2, BrowserCode.Bang, BrowserCode.OnBrowserLite,
        BrowserCode.DiigoBrowser, BrowserCode.TrueLocationBrowser, BrowserCode.MixerBoxAi, BrowserCode.YouBrowser,
        BrowserCode.MaxBrowser, BrowserCode.LeganBrowser, BrowserCode.OjrBrowser, BrowserCode.InvoltaGo,
        BrowserCode.HabitBrowser, BrowserCode.OwlBrowser, BrowserCode.Orbitum, BrowserCode.Photon,
        BrowserCode.KeyboardBrowser, BrowserCode.StealthBrowser, BrowserCode.TalkTo, BrowserCode.Proxynet,
        BrowserCode.GoodBrowser, BrowserCode.Proxyium, BrowserCode.Vuhuv, BrowserCode.FireBrowser,
        BrowserCode.LightningBrowserPlus, BrowserCode.DarkWeb, BrowserCode.DarkWebPrivate, BrowserCode.SkyLeap,
        BrowserCode.Kitt, BrowserCode.NookBrowser, BrowserCode.Kun, BrowserCode.WukongBrowser,
        BrowserCode.MotorolaInternetBrowser, BrowserCode.UPhoneBrowser, BrowserCode.ZteBrowser,
        BrowserCode.Presearch, BrowserCode.Ninesky, BrowserCode.Veera, BrowserCode.PintarBrowser,
        BrowserCode.BrowserMini, BrowserCode.FossBrowser, BrowserCode.PeachBrowser, BrowserCode.AppTecSecureBrowser,
        BrowserCode.ProxyFox, BrowserCode.ProxyMax, BrowserCode.KeepSolidBrowser, BrowserCode.OnionBrowser2,
        BrowserCode.AiBrowser, BrowserCode.HaloBrowser, BrowserCode.MmboxXBrowser, BrowserCode.XnBrowse,
        BrowserCode.OpenBrowserLite, BrowserCode.PuffinIncognitoBrowser, BrowserCode.PuffinCloudBrowser,
        BrowserCode.PrivacyPioneerBrowser, BrowserCode.Pluma, BrowserCode.PocketInternetExplorer
    }.ToFrozenSet();

    private static readonly FrozenDictionary<string, FrozenSet<string>> ClientHintBrandMapping =
        new Dictionary<string, FrozenSet<string>>
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

    private static readonly FrozenSet<BrowserCode> PriorityBrowsers = new[]
    {
        BrowserCode.Atom, BrowserCode.AlohaBrowser, BrowserCode.HuaweiBrowser, BrowserCode.OjrBrowser,
        BrowserCode.MiBrowser, BrowserCode.OperaMobile, BrowserCode.Opera, BrowserCode.Veera,
    }.ToFrozenSet();

    private static readonly FrozenSet<BrowserCode> ChromiumBrowsers = new[]
    {
        BrowserCode.Chromium, BrowserCode.ChromeWebview, BrowserCode.AndroidBrowser,
    }.ToFrozenSet();

    private static readonly Regex ChromeSafariRegex =
        new(@"Chrome/.+ Safari/537\.36", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex CypressOrPhantomJsRegex = new("Cypress|PhantomJS", RegexOptions.Compiled);

    private static readonly Regex IridiumVersionRegex = new("^202[0-4]", RegexOptions.Compiled);


    static BrowserParser()
    {
        var duplicateCompactNames = BrowserCodeMapping.Values
            .Select(x => x.RemoveSpaces())
            .GroupBy(x => x)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        var mapping = new Dictionary<string, string>();

        foreach (var name in BrowserNameMapping.Keys)
        {
            var compactName = name.RemoveSpaces();

            if (!duplicateCompactNames.Contains(compactName))
            {
                mapping.Add(compactName, name);
            }
        }

        CompactToFullNameMapping = mapping.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }

    public BrowserParser(ParserOptions? parserOptions = null)
    {
        _parserOptions = parserOptions ?? new ParserOptions();
        _clientParser = new ClientParser(_parserOptions);
    }

    private static string ApplyClientHintBrandMapping(string brand)
    {
        foreach (var clientHint in ClientHintBrandMapping)
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
        if (BrowserNameMapping.TryGetValue(name, out var code))
        {
            return TryMapCodeToFamily(code, out result);
        }

        result = null;
        return false;
    }

    private static bool TryMapCodeToFamily(BrowserCode code, [NotNullWhen((true))] out string? result)
    {
        foreach (var browserFamily in BrowserFamilyMapping)
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

    private static string? BuildEngine(string userAgent, Engine? engine, string? browserVersion)
    {
        var result = engine?.Default;

        if (engine?.Versions?.Count > 0 && !string.IsNullOrEmpty(browserVersion))
        {
            foreach (var version in engine.Versions)
            {
                if (ParserExtensions.TryCompareVersions(browserVersion, version.Key, out var comparisonResult) &&
                    comparisonResult < 0)
                {
                    continue;
                }

                result = version.Value;
            }
        }

        if (string.IsNullOrEmpty(result))
        {
            EngineParser.TryParse(userAgent, out result);
        }

        return result;
    }

    private string? BuildEngineVersion(string userAgent, string? engine)
    {
        if (string.IsNullOrEmpty(engine))
        {
            return null;
        }

        EngineVersionParser.TryParse(userAgent, engine, out var result);
        return ParserExtensions.BuildVersion(result, _parserOptions.VersionTruncation);

    }

    private static bool TryGetBrowserCode(string browserName, [NotNullWhen(true)] out BrowserCode? result)
    {
        browserName = browserName.CollapseSpaces();
        var hasBrowserSuffix = browserName.EndsWith("Browser");

        if (BrowserNameMapping.TryGetValue(browserName, out var browserCode) ||
            (hasBrowserSuffix && BrowserNameMapping.TryGetValue(browserName[..^7].TrimEnd(), out browserCode)) ||
            (!hasBrowserSuffix && BrowserNameMapping.TryGetValue($"{browserName} Browser", out browserCode)) ||
            (CompactToFullNameMapping.TryGetValue(browserName.RemoveSpaces(), out var fullName) &&
             BrowserNameMapping.TryGetValue(fullName, out browserCode)))
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

        string? name = null, version = null;
        BrowserCode? code = null;

        foreach (var fullVersion in clientHints.FullVersionList)
        {
            var browserName = ApplyClientHintBrandMapping(fullVersion.Key);

            if (TryGetBrowserCode(browserName, out var browserCode))
            {
                name = BrowserCodeMapping[browserCode.Value];
                code = browserCode;
                version = fullVersion.Value;
            }

            // Exit if the detected browser brand is not Chromium or Microsoft Edge, otherwise, continue searching.
            if (!string.IsNullOrEmpty(name) && name != BrowserNames.Chromium && name != BrowserNames.MicrosoftEdge)
            {
                break;
            }
        }

        if (string.IsNullOrEmpty(name) || !code.HasValue)
        {
            result = null;
            return false;
        }

        if (!string.IsNullOrEmpty(clientHints.UaFullVersion))
        {
            version = clientHints.UaFullVersion;
        }

        result = new ClientHintsBrowserInfo
        {
            Name = name,
            Code = code.Value,
            Version = ParserExtensions.BuildVersion(version, _parserOptions.VersionTruncation),
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

        foreach (var browserPattern in Browsers)
        {
            match = browserPattern.Regex.Match(userAgent);

            if (match.Success)
            {
                browser = browserPattern;
                break;
            }
        }

        if (browser is null || match is null || !match.Success)
        {
            result = null;
            return false;
        }

        string name = ParserExtensions.FormatWithMatch(browser.Name, match);

        if (BrowserNameMapping.TryGetValue(name, out var code))
        {
            var version = ParserExtensions.BuildVersion(browser.Version, match, _parserOptions.VersionTruncation);
            var engine = BuildEngine(userAgent, browser.Engine, version);
            var engineVersion = BuildEngineVersion(userAgent, engine);

            result = new UserAgentBrowserInfo
            {
                Name = name,
                Code = code,
                Version = version,
                Engine = engine,
                EngineVersion = engineVersion
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
        return shortVersion.Length < fullVersion.Length &&
               ParserExtensions.TryCompareVersions(shortVersion, fullVersion, out var comparisonResult) &&
               comparisonResult == 0;
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
        if (!_parserOptions.DisableBotDetection && BotParser.IsBot(userAgent))
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

        return TryParse(userAgent, clientHints, out result);
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

        if (TryParseBrowserFromClientHints(clientHints, out var browserFromClientHints) &&
            !string.IsNullOrEmpty(browserFromClientHints.Version))
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
                if (!string.IsNullOrEmpty(browserFromUserAgent.Version) && version.StartsWith("15") &&
                    browserFromUserAgent.Version.StartsWith("114"))
                {
                    name = BrowserNames.SecureBrowser360;
                    code = BrowserCode.SecureBrowser360;
                    engine = browserFromUserAgent.Engine;
                    engineVersion = browserFromUserAgent.EngineVersion;
                }

                if (!string.IsNullOrEmpty(browserFromUserAgent.Version) &&
                    (PriorityBrowsers.Contains(code.Value) ||
                     IsSameTruncatedVersion(version, browserFromUserAgent.Version)))
                {
                    version = browserFromUserAgent.Version;
                }

                if (name == BrowserNames.VewdBrowser)
                {
                    engine = browserFromUserAgent.Engine;
                    engineVersion = browserFromUserAgent.EngineVersion;
                }

                if (name is BrowserNames.Chromium or BrowserNames.ChromeWebview &&
                    !ChromiumBrowsers.Contains(browserFromUserAgent.Code))
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

                if (name != browserFromUserAgent.Name && TryMapNameToFamily(name, out var familyFromName) &&
                    TryMapNameToFamily(browserFromUserAgent.Name, out var familyFromUserAgent) &&
                    familyFromName == familyFromUserAgent)
                {
                    engine = browserFromUserAgent.Engine;
                    engineVersion = browserFromUserAgent.EngineVersion;
                }

                if (name == browserFromUserAgent.Name)
                {
                    engine = browserFromUserAgent.Engine;
                    engineVersion = browserFromUserAgent.EngineVersion;
                }

                if (!string.IsNullOrEmpty(browserFromUserAgent.Version) && !string.IsNullOrEmpty(version) &&
                    browserFromUserAgent.Version.StartsWith(version) &&
                    ParserExtensions.TryCompareVersions(version, browserFromUserAgent.Version,
                        out var comparisonResult) && comparisonResult < 0)
                {
                    version = browserFromUserAgent.Version;
                }

                if (name == BrowserNames.DuckDuckGoPrivacyBrowser)
                {
                    version = null;
                }

                if (engine == BrowserEngines.Blink && name != BrowserNames.Iridium &&
                    !string.IsNullOrEmpty(engineVersion) &&
                    ParserExtensions.TryCompareVersions(engineVersion, browserFromClientHints.Version,
                        out comparisonResult) && comparisonResult < 0)
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

        if (code.HasValue)
        {
            TryMapCodeToFamily(code.Value, out family);
        }

        if (BrowserHintParser.TryParseBrowserName(clientHints, out var browserName) && name != browserName)
        {
            version = null;

            if (BrowserNameMapping.TryGetValue(browserName, out var browserCode))
            {
                name = browserName;
                code = browserCode;
            }

            if (ChromeSafariRegex.IsMatch(userAgent))
            {
                engine = BrowserEngines.Blink;
                engineVersion = BuildEngineVersion(userAgent, engine);

                if (code.HasValue)
                {
                    family = TryMapCodeToFamily(code.Value, out family) ? family : BrowserFamilies.Chrome;
                }
            }
        }

        if (string.IsNullOrEmpty(name) || CypressOrPhantomJsRegex.IsMatch(userAgent) || !code.HasValue)
        {
            result = null;
            return false;
        }

        switch (name)
        {
            // Ignore "Blink" engine version for "Flow Browser".
            case BrowserNames.FlowBrowser when engine == BrowserEngines.Blink:
                engineVersion = null;
                break;
            // "Every Browser" mimics a Chrome user agent on Android.
            // "TV-Browser Internet" mimics a Firefox user agent.
            case BrowserNames.EveryBrowser:
            case BrowserNames.TvBrowserInternet when engine == BrowserEngines.Gecko:
                family = BrowserFamilies.Chrome;
                engine = BrowserEngines.Blink;
                engineVersion = null;
                break;
            case BrowserNames.Wolvic when engine == BrowserEngines.Blink:
                family = BrowserFamilies.Chrome;
                break;
            case BrowserNames.Wolvic when engine == BrowserEngines.Gecko:
                family = BrowserFamilies.Firefox;
                break;
        }

        result = new BrowserInfo
        {
            Name = name,
            Code = code.Value,
            Family = family,
            Version = version,
            Engine = string.IsNullOrEmpty(engine) && string.IsNullOrEmpty(engineVersion)
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
