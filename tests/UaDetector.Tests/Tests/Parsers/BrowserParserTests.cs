using Shouldly;

using UaDetector.Models.Constants;
using UaDetector.Models.Enums;
using UaDetector.Parsers;
using UaDetector.Results;

using UaDetector.Tests.Fixtures.Models;
using UaDetector.Tests.Helpers;
using UaDetector.Utils;

namespace UaDetector.Tests.Tests.Parsers;

public class BrowserParserTests
{
    [Test]
    public void BrowserParser_Instantiation_ShouldNotThrowException()
    {
        Should.NotThrow(() => new BrowserParser());
    }

    [Test]
    public void BrowserParser_ShouldImplement_IBrowserParser()
    {
        var parser = new BrowserParser();
        parser.ShouldBeAssignableTo<IBrowserParser>();
    }

    [Test]
    public void Browsers_ShouldContainKeysForAllBrowserNames()
    {
        var browserNames = BrowserParser.Browsers
            .Select(browser => browser.Name);

        foreach (var browserName in browserNames)
        {
            BrowserParser.BrowserNameMapping.ShouldContainKey(browserName);
        }
    }

    [Test]
    public void BrowserNameMappings_ShouldMatchParsedBrowserNames()
    {
        var ignoredNames = new List<string>
        {
            BrowserNames.LujoTvBrowser,
            BrowserNames.QuickBrowser,
            BrowserNames.FrostPlus,
            BrowserNames.OfficeBrowser,
            BrowserNames.TvBrowserInternet,
            BrowserNames.IncognitoBrowser,
            BrowserNames.NakedBrowserPro,
            BrowserNames.OpusBrowser,
            BrowserNames.LynketBrowser,
            BrowserNames.ProxyMax,
            BrowserNames.VewdBrowser,
            BrowserNames.GodzillaBrowser,
            BrowserNames.CakeBrowser,
            BrowserNames.OhPrivateBrowser,
            BrowserNames.SavannahBrowser,
            BrowserNames.AdultBrowser,
            BrowserNames.LogicUiTvBrowser,
            BrowserNames.StealthBrowser,
            BrowserNames.Pluma,
            BrowserNames.ComfortBrowser,
            BrowserNames.OpenBrowser4U,
            BrowserNames.TintBrowser,
            BrowserNames.SoundyBrowser,
            BrowserNames.UiBrowserMini,
            BrowserNames.LightningBrowserPlus,
            BrowserNames.HexaWebBrowser,
            BrowserNames.Chowbo,
            BrowserNames.FastExplorer,
            BrowserNames.DebuggableBrowser,
            BrowserNames.TotalBrowser,
            BrowserNames.TalkTo,
            BrowserNames.SilverMobUs,
            BrowserNames.FreedomBrowser,
            BrowserNames.HubBrowser,
            BrowserNames.IndianUcMiniBrowser,
            BrowserNames.PeachBrowser,
            BrowserNames.WebBrowserAndExplorer,
            BrowserNames.OpenTvBrowser,
            BrowserNames.EinkBro,
            BrowserNames.DarkWebBrowser,
            BrowserNames.XnxBrowser,
            BrowserNames.PrivacyPioneerBrowser,
            BrowserNames.WaveBrowser,
            BrowserNames.SavySoda,
            BrowserNames.TorBrowser,
            BrowserNames.XBrowserProSuperFast,
            BrowserNames.OpenBrowserFast5G,
            BrowserNames.IeBrowserFast,
            BrowserNames.KeepSolidBrowser,
            BrowserNames.BelvaBrowser,
            BrowserNames.Sidekick,
            BrowserNames.GoBrowser2,
            BrowserNames.VividBrowserMini,
            BrowserNames.Fulldive,
            BrowserNames.TucMiniBrowser,
            BrowserNames.CmMini,
            BrowserNames.DarkBrowser,
            BrowserNames.MaxTubeBrowser,
            BrowserNames.BrowserHupPro,
            BrowserNames.VegasBrowser,
            BrowserNames.PrivacyExplorerFastSafe,
            BrowserNames.FastBrowserUcLite,
            BrowserNames.HtcBrowser,
            BrowserNames.Catsxp,
            BrowserNames.SweetBrowser,
            BrowserNames.Via,
            BrowserNames.SamsungBrowserLite,
            BrowserNames.Browser1DmPlus,
            BrowserNames.XtremeCast,
            BrowserNames.InBrowser,
            BrowserNames.InstaBrowser,
            BrowserNames.KeyboardBrowser,
            BrowserNames.NakedBrowser,
            BrowserNames.CrowBrowser,
            BrowserNames.BrowsBit,
            BrowserNames.BeyondPrivateBrowser,
            BrowserNames.OdinBrowser,
            BrowserNames.YoBrowser,
            BrowserNames.OpenBrowserLite,
            BrowserNames.OnionBrowser2,
            BrowserNames.MmboxXBrowser,
            BrowserNames.MidoriLite,
            BrowserNames.NovaVideoDownloaderPro,
            BrowserNames.OrNetBrowser,
            BrowserNames.MarsLabWebBrowser,
            BrowserNames.LightningBrowser,
            BrowserNames.BitchuteBrowser,
            BrowserNames.SunflowerBrowser,
            BrowserNames.PureLiteBrowser,
            BrowserNames.FloatBrowser,
            BrowserNames.PrivateInternetBrowser,
            BrowserNames.AiBrowser,
            BrowserNames.Frost,
            BrowserNames.EasyBrowser,
            BrowserNames.PhantomMe,
            BrowserNames.Atlas,
            BrowserNames.Proxynet,
            BrowserNames.AsusBrowser,
            BrowserNames.Proxyium,
            BrowserNames.DarkWeb,
            BrowserNames.HollaWebBrowser,
            BrowserNames.PureMiniBrowser,
            BrowserNames.HabitBrowser,
            BrowserNames.ExploreBrowser,
            BrowserNames.LarkBrowser,
            BrowserNames.ZordoBrowser,
            BrowserNames.IDesktopPcBrowser,
            BrowserNames.DesiBrowser,
            BrowserNames.OhBrowser,
            BrowserNames.Amerigo,
            BrowserNames.WorldBrowser,
            BrowserNames.DarkWebPrivate,
            BrowserNames.PronHubBrowser,
            BrowserNames.XBrowserMini,
            BrowserNames.BrowserMini,
            BrowserNames.OceanBrowser,
            BrowserNames.Browser1Dm,
            BrowserNames.QjyTvBrowser,
            BrowserNames.SharkeeBrowser,
            BrowserNames.ZircoBrowser,
            BrowserNames.Flyperlink,
            BrowserNames.KutoMiniBrowser,
            BrowserNames.Pawxy,
            BrowserNames.OwlBrowser,
            BrowserNames.CherryBrowser,
            BrowserNames.BroKeepBrowser,
            BrowserNames.DucBrowser,
            BrowserNames.RaiseFastBrowser,
            BrowserNames.Orbitum,
            BrowserNames.FireBrowser,
            BrowserNames.OpenBrowser,
            BrowserNames.YuzuBrowser,
            BrowserNames.FirefoxKlar,
            BrowserNames.Wavebox,
            BrowserNames.SxBrowser,
            BrowserNames.AnkaBrowser,
            BrowserNames.InternetWebbrowser,
            BrowserNames.SonySmallBrowser,
            BrowserNames.SuperFastBrowser,
            BrowserNames.SuperFastBrowser2,
            BrowserNames.InvoltaGo,
            BrowserNames.CgBrowser,
            BrowserNames.XVpn,
            BrowserNames.BerryBrowser,
            BrowserNames.Mises,
            BrowserNames.OperaCrypto,
            BrowserNames.Spark,
            BrowserNames.ApusBrowser,
            BrowserNames.FossBrowser,
            BrowserNames.BasicWebBrowser,
            BrowserNames.DragonBrowser,
            BrowserNames.HaloBrowser,
            BrowserNames.MeBrowser,
            BrowserNames.Cromite,
            BrowserNames.PintarBrowser,
            BrowserNames.MmxBrowser,
            BrowserNames.UmeBrowser,
            BrowserNames.InternetBrowserSecure,
            BrowserNames.KidsSafeBrowser,
            BrowserNames.SotiSurf,
            BrowserNames.HerondBrowser,
            BrowserNames.BxeBrowser,
            BrowserNames.GoodBrowser,
            BrowserNames.VBrowser,
            BrowserNames.Thor,
            BrowserNames.BlackLionBrowser,
            BrowserNames.EveryBrowser,
            BrowserNames.XBrowserLite,
            BrowserNames.BfBrowser,
            BrowserNames.Qmamu,
            BrowserNames.XBrowser,
            BrowserNames.CaveBrowser,
            BrowserNames.SmartBrowser,
            BrowserNames.Photon,
            BrowserNames.AzkaBrowser,
            BrowserNames.IvviBrowser,
            BrowserNames.SmartSearchAndWebBrowser,
            BrowserNames.Vuhuv,
            BrowserNames.NomOneVrBrowser,
            BrowserNames.XoolooInternet,
            BrowserNames.RabbitPrivateBrowser,
            BrowserNames.NextWordBrowser,
            BrowserNames.BrowspeedBrowser,
            BrowserNames.AppBrowzer,
            BrowserNames.AmazeBrowser,
            BrowserNames.PuffinIncognitoBrowser,
            BrowserNames.GBrowser,
            BrowserNames.Jelly,
            BrowserNames.FieryBrowser,
            BrowserNames.Gener8,
            BrowserNames.DolphinZero,
            BrowserNames.ArmorflyBrowser
        };

        var browserNames = BrowserParser.Browsers
            .Select(browser => browser.Name)
            .ToHashSet();

        foreach (var name in BrowserParser.BrowserNameMapping.Keys.Except(ignoredNames))
        {
            browserNames.ShouldContain(name);
        }
    }

    [Test]
    public void BrowserCodeMapping_ShouldContainAllBrowserCodes()
    {
        foreach (BrowserCode browserCode in Enum.GetValues(typeof(BrowserCode)))
        {
            BrowserParser.BrowserCodeMapping.ShouldContainKey(browserCode);
        }
    }

    [Test]
    public void CompactToFullNameMapping_ShouldContainKeyForAllUniqueNames()
    {
        var duplicateCompactNames = BrowserParser.BrowserCodeMapping.Values
            .Select(x => x.RemoveSpaces())
            .GroupBy(x => x)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        var browserNames = new List<string>();

        foreach (var name in BrowserParser.BrowserNameMapping.Keys)
        {
            var compactName = name.RemoveSpaces();

            if (!duplicateCompactNames.Contains(compactName))
            {
                browserNames.Add(compactName);
            }
        }

        foreach (var name in browserNames)
        {
            BrowserParser.CompactToFullNameMapping.ShouldContainKey(name);
        }
    }

    [Test]
    public async Task TryParse_WithFixtureData_ShouldReturnExpectedBrowserInfo()
    {
        var fixturePath = Path.Combine("Fixtures", "Resources", "browsers.json");
        var fixtures = await FixtureLoader.LoadAsync<BrowserFixture>(fixturePath);

        var parser = new BrowserParser(VersionTruncation.None);

        foreach (var fixture in fixtures)
        {
            BrowserInfo? result;

            if (fixture.Headers is null)
            {
                parser.TryParse(fixture.UserAgent, out result).ShouldBeTrue();
            }
            else
            {
                parser.TryParse(fixture.UserAgent, fixture.Headers, out result).ShouldBeTrue();
            }

            result.ShouldNotBeNull();
            result.Name.ShouldBe(fixture.Browser.Name);
            result.Code.ShouldBe(fixture.Browser.Code);
            result.Version.ShouldBe(fixture.Browser.Version);
            result.Family.ShouldBe(fixture.Browser.Family);
        }
    }
}
