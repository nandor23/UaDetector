using System.Collections.Frozen;

using UADetector.Models;
using UADetector.Regexes.Models;
using UADetector.Results;
using UADetector.Utils;

namespace UADetector.Parsers;

internal sealed class OperatingSystemParser
{
    private const string ResourceName = "Regexes.Resources.operating_systems.yml";

    private static readonly IEnumerable<Os> OsRegexes =
        ParserExtensions.LoadRegexes<Os>(ResourceName, RegexPatternType.UserAgent);

    private static readonly FrozenDictionary<OsCode, string?> OsCodeMapping =
        new Dictionary<OsCode, string?>
        {
            { OsCode.AIX, OsNames.Aix },
            { OsCode.AND, OsNames.Android },
            { OsCode.ADR, OsNames.AndroidTv },
            { OsCode.ALP, OsNames.AlpineLinux },
            { OsCode.AMZ, OsNames.AmazonLinux },
            { OsCode.AMG, OsNames.AmigaOs },
            { OsCode.ARM, OsNames.ArmadilloOs },
            { OsCode.ARO, OsNames.Aros },
            { OsCode.ATV, OsNames.TvOs },
            { OsCode.ARL, OsNames.ArchLinux },
            { OsCode.AOS, OsNames.AosCos },
            { OsCode.ASP, OsNames.AspLinux },
            { OsCode.AZU, OsNames.AzureLinux },
            { OsCode.BTR, OsNames.BackTrack },
            { OsCode.SBA, OsNames.Bada },
            { OsCode.BYI, OsNames.BaiduYi },
            { OsCode.BEO, OsNames.BeOs },
            { OsCode.BLB, OsNames.BlackBerryOs },
            { OsCode.QNX, OsNames.BlackBerryTabletOs },
            { OsCode.PAN, OsNames.BlackPantherOs },
            { OsCode.BOS, OsNames.BlissOs },
            { OsCode.BMP, OsNames.Brew },
            { OsCode.BSN, OsNames.BrightSignOs },
            { OsCode.CAI, OsNames.CaixaMagica },
            { OsCode.CES, OsNames.CentOs },
            { OsCode.CST, OsNames.CentOsStream },
            { OsCode.CLO, OsNames.ClearLinuxOs },
            { OsCode.CLR, OsNames.ClearOsMobile },
            { OsCode.COS, OsNames.ChromeOs },
            { OsCode.CRS, OsNames.ChromiumOs },
            { OsCode.CHN, OsNames.ChinaOs },
            { OsCode.COL, OsNames.CoolitaOs },
            { OsCode.CYN, OsNames.CyanogenMod },
            { OsCode.DEB, OsNames.Debian },
            { OsCode.DEE, OsNames.Deepin },
            { OsCode.DFB, OsNames.DragonFly },
            { OsCode.DVK, OsNames.DvkBuntu },
            { OsCode.ELE, OsNames.ElectroBsd },
            { OsCode.EUL, OsNames.EulerOs },
            { OsCode.FED, OsNames.Fedora },
            { OsCode.FEN, OsNames.Fenix },
            { OsCode.FOS, OsNames.FirefoxOs },
            { OsCode.FIR, OsNames.FireOs },
            { OsCode.FOR, OsNames.ForesightLinux },
            { OsCode.FRE, OsNames.Freebox },
            { OsCode.BSD, OsNames.FreeBsd },
            { OsCode.FRI, OsNames.FritzOs },
            { OsCode.FYD, OsNames.FydeOs },
            { OsCode.FUC, OsNames.Fuchsia },
            { OsCode.GNT, OsNames.Gentoo },
            { OsCode.GNX, OsNames.Genix },
            { OsCode.GEO, OsNames.Geos },
            { OsCode.GNS, OsNames.GNewSense },
            { OsCode.GRI, OsNames.GridOs },
            { OsCode.GTV, OsNames.GoogleTv },
            { OsCode.HPX, OsNames.HpUx },
            { OsCode.HAI, OsNames.HaikuOs },
            { OsCode.IPA, OsNames.IPadOs },
            { OsCode.HAR, OsNames.HarmonyOs },
            { OsCode.HAS, OsNames.HasCodingOs },
            { OsCode.HEL, OsNames.HelixOs },
            { OsCode.IRI, OsNames.Irix },
            { OsCode.INF, OsNames.Inferno },
            { OsCode.JME, OsNames.JavaMe },
            { OsCode.JOL, OsNames.JoliOs },
            { OsCode.KOS, OsNames.KaiOs },
            { OsCode.KAL, OsNames.Kali },
            { OsCode.KAN, OsNames.Kanotix },
            { OsCode.KIN, OsNames.Kinos },
            { OsCode.KNO, OsNames.Knoppix },
            { OsCode.KTV, OsNames.KreaTv },
            { OsCode.KBT, OsNames.Kubuntu },
            { OsCode.LIN, OsNames.GnuLinux },
            { OsCode.LEA, OsNames.LeafOs },
            { OsCode.LND, OsNames.LindowsOs },
            { OsCode.LNS, OsNames.Linspire },
            { OsCode.LEN, OsNames.LineageOs },
            { OsCode.LIR, OsNames.LiriOs },
            { OsCode.LOO, OsNames.Loongnix },
            { OsCode.LBT, OsNames.Lubuntu },
            { OsCode.LOS, OsNames.LuminOs },
            { OsCode.LUN, OsNames.LuneOs },
            { OsCode.VLN, OsNames.VectorLinux },
            { OsCode.MAC, OsNames.Mac },
            { OsCode.MAE, OsNames.Maemo },
            { OsCode.MAG, OsNames.Mageia },
            { OsCode.MDR, OsNames.Mandriva },
            { OsCode.SMG, OsNames.MeeGo },
            { OsCode.MET, OsNames.MetaHorizon },
            { OsCode.MCD, OsNames.MocorDroid },
            { OsCode.MON, OsNames.MoonOs },
            { OsCode.EZX, OsNames.MotorolaEzx },
            { OsCode.MIN, OsNames.Mint },
            { OsCode.MLD, OsNames.MildWild },
            { OsCode.MOR, OsNames.MorphOs },
            { OsCode.NBS, OsNames.NetBsd },
            { OsCode.MTK, OsNames.MtkNucleus },
            { OsCode.MRE, OsNames.Mre },
            { OsCode.NXT, OsNames.NeXtStep },
            { OsCode.NWS, OsNames.NewsOs },
            { OsCode.WII, OsNames.Nintendo },
            { OsCode.NDS, OsNames.NintendoMobile },
            { OsCode.NOV, OsNames.Nova },
            { OsCode.OS2, OsNames.Os2 },
            { OsCode.T64, OsNames.Osf1 },
            { OsCode.OBS, OsNames.OpenBsd },
            { OsCode.OVS, OsNames.OpenVms },
            { OsCode.OVZ, OsNames.OpenVz },
            { OsCode.OWR, OsNames.OpenWrt },
            { OsCode.OTV, OsNames.OperaTv },
            { OsCode.ORA, OsNames.OracleLinux },
            { OsCode.ORD, OsNames.Ordissimo },
            { OsCode.PAR, OsNames.Pardus },
            { OsCode.PCL, OsNames.PcLinuxOs },
            { OsCode.PIC, OsNames.PicoOs },
            { OsCode.PLA, OsNames.PlasmaMobile },
            { OsCode.PSP, OsNames.PlayStationPortable },
            { OsCode.PS3, OsNames.PlayStation },
            { OsCode.PVE, OsNames.ProxmoxVe },
            { OsCode.PUF, OsNames.PuffinOs },
            { OsCode.PUR, OsNames.PureOs },
            { OsCode.QTP, OsNames.Qtopia },
            { OsCode.PIO, OsNames.RaspberryPiOs },
            { OsCode.RAS, OsNames.Raspbian },
            { OsCode.RHT, OsNames.RedHat },
            { OsCode.RST, OsNames.RedStar },
            { OsCode.RED, OsNames.RedOs },
            { OsCode.REV, OsNames.RevengeOs },
            { OsCode.RIS, OsNames.RisingOs },
            { OsCode.ROS, OsNames.RiscOs },
            { OsCode.ROC, OsNames.RockyLinux },
            { OsCode.ROK, OsNames.RokuOs },
            { OsCode.RSO, OsNames.Rosa },
            { OsCode.ROU, OsNames.RouterOs },
            { OsCode.REM, OsNames.RemixOs },
            { OsCode.RRS, OsNames.ResurrectionRemixOs },
            { OsCode.REX, OsNames.Rex },
            { OsCode.RZD, OsNames.RazoDroid },
            { OsCode.RXT, OsNames.RtosNext },
            { OsCode.SAB, OsNames.Sabayon },
            { OsCode.SSE, OsNames.Suse },
            { OsCode.SAF, OsNames.SailfishOs },
            { OsCode.SCI, OsNames.ScientificLinux },
            { OsCode.SEE, OsNames.SeewoOs },
            { OsCode.SER, OsNames.SerenityOs },
            { OsCode.SIR, OsNames.SirinOs },
            { OsCode.SLW, OsNames.Slackware },
            { OsCode.SOS, OsNames.Solaris },
            { OsCode.SBL, OsNames.StarBladeOs },
            { OsCode.SYL, OsNames.Syllable },
            { OsCode.SYM, OsNames.Symbian },
            { OsCode.SYS, OsNames.SymbianOs },
            { OsCode.S40, OsNames.SymbianOsSeries40 },
            { OsCode.S60, OsNames.SymbianOsSeries60 },
            { OsCode.SY3, OsNames.Symbian3 },
            { OsCode.TEN, OsNames.TencentOs },
            { OsCode.TDX, OsNames.ThreadX },
            { OsCode.TIZ, OsNames.Tizen },
            { OsCode.TIV, OsNames.TiVoOs },
            { OsCode.TOS, OsNames.TmaxOs },
            { OsCode.TUR, OsNames.Turbolinux },
            { OsCode.UBT, OsNames.Ubuntu },
            { OsCode.ULT, OsNames.Ultrix },
            { OsCode.UOS, OsNames.Uos },
            { OsCode.VID, OsNames.Vidae },
            { OsCode.VIZ, OsNames.ViziOs },
            { OsCode.WAS, OsNames.WatchOs },
            { OsCode.WER, OsNames.WearOs },
            { OsCode.WTV, OsNames.WebTv },
            { OsCode.WHS, OsNames.WhaleOs },
            { OsCode.WIN, OsNames.Windows },
            { OsCode.WCE, OsNames.WindowsCe },
            { OsCode.WIO, OsNames.WindowsIoT },
            { OsCode.WMO, OsNames.WindowsMobile },
            { OsCode.WPH, OsNames.WindowsPhone },
            { OsCode.WRT, OsNames.WindowsRt },
            { OsCode.WPO, OsNames.WoPhone },
            { OsCode.XBX, OsNames.Xbox },
            { OsCode.XBT, OsNames.Xubuntu },
            { OsCode.YNS, OsNames.YunOs },
            { OsCode.ZEN, OsNames.Zenwalk },
            { OsCode.ZOR, OsNames.ZorinOs },
            { OsCode.IOS, OsNames.IOs },
            { OsCode.POS, OsNames.PalmOs },
            { OsCode.WEB, OsNames.Webian },
            { OsCode.WOS, OsNames.WebOs },
        }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, OsCode?> OsNameMapping = OsCodeMapping
        .ToDictionary(e => e.Value, e => (OsCode?)e.Key).ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    private static readonly FrozenDictionary<string, FrozenSet<OsCode>> OsFamilyMapping =
        new Dictionary<string, FrozenSet<OsCode>>
        {
            {
                OsFamilies.Android,
                new[]
                {
                    OsCode.AND, OsCode.CYN, OsCode.FIR, OsCode.REM, OsCode.RZD,
                    OsCode.MLD, OsCode.MCD, OsCode.YNS, OsCode.GRI, OsCode.HAR,
                    OsCode.ADR, OsCode.CLR, OsCode.BOS, OsCode.REV, OsCode.LEN,
                    OsCode.SIR, OsCode.RRS, OsCode.WER, OsCode.PIC, OsCode.ARM,
                    OsCode.HEL, OsCode.BYI, OsCode.RIS, OsCode.PUF, OsCode.LEA, OsCode.MET
                }.ToFrozenSet()
            },
            { OsFamilies.AmigaOs, new[] { OsCode.AMG, OsCode.MOR, OsCode.ARO }.ToFrozenSet() },
            { OsFamilies.BlackBerry, new[] { OsCode.BLB, OsCode.QNX }.ToFrozenSet() },
            { OsFamilies.Brew, new[] { OsCode.BMP }.ToFrozenSet() },
            { OsFamilies.BeOs, new[] { OsCode.BEO, OsCode.HAI }.ToFrozenSet() },
            {
                OsFamilies.ChromeOs,
                new[] { OsCode.COS, OsCode.CRS, OsCode.FYD, OsCode.SEE }.ToFrozenSet()
            },
            { OsFamilies.FirefoxOs, new[] { OsCode.FOS, OsCode.KOS }.ToFrozenSet() },
            { OsFamilies.GamingConsole, new[] { OsCode.WII, OsCode.PS3 }.ToFrozenSet() },
            { OsFamilies.GoogleTv, new[] { OsCode.GTV }.ToFrozenSet() },
            { OsFamilies.Ibm, new[] { OsCode.OS2 }.ToFrozenSet() },
            {
                OsFamilies.Ios,
                new[] { OsCode.IOS, OsCode.ATV, OsCode.WAS, OsCode.IPA }.ToFrozenSet()
            },
            { OsFamilies.RiscOs, new[] { OsCode.ROS }.ToFrozenSet() },
            {
                OsFamilies.GnuLinux,
                new[]
                {
                    OsCode.LIN, OsCode.ARL, OsCode.DEB, OsCode.KNO, OsCode.MIN,
                    OsCode.UBT, OsCode.KBT, OsCode.XBT, OsCode.LBT, OsCode.FED,
                    OsCode.RHT, OsCode.VLN, OsCode.MDR, OsCode.GNT, OsCode.SAB,
                    OsCode.SLW, OsCode.SSE, OsCode.CES, OsCode.BTR, OsCode.SAF,
                    OsCode.ORD, OsCode.TOS, OsCode.RSO, OsCode.DEE, OsCode.FRE,
                    OsCode.MAG, OsCode.FEN, OsCode.CAI, OsCode.PCL, OsCode.HAS,
                    OsCode.LOS, OsCode.DVK, OsCode.ROK, OsCode.OWR, OsCode.OTV,
                    OsCode.KTV, OsCode.PUR, OsCode.PLA, OsCode.FUC, OsCode.PAR,
                    OsCode.FOR, OsCode.MON, OsCode.KAN, OsCode.ZEN, OsCode.LND,
                    OsCode.LNS, OsCode.CHN, OsCode.AMZ, OsCode.TEN, OsCode.CST,
                    OsCode.NOV, OsCode.ROU, OsCode.ZOR, OsCode.RED, OsCode.KAL,
                    OsCode.ORA, OsCode.VID, OsCode.TIV, OsCode.BSN, OsCode.RAS,
                    OsCode.UOS, OsCode.PIO, OsCode.FRI, OsCode.LIR, OsCode.WEB,
                    OsCode.SER, OsCode.ASP, OsCode.AOS, OsCode.LOO, OsCode.EUL,
                    OsCode.SCI, OsCode.ALP, OsCode.CLO, OsCode.ROC, OsCode.OVZ,
                    OsCode.PVE, OsCode.RST, OsCode.EZX, OsCode.GNS, OsCode.JOL,
                    OsCode.TUR, OsCode.QTP, OsCode.WPO, OsCode.PAN, OsCode.VIZ,
                    OsCode.AZU, OsCode.COL
                }.ToFrozenSet()
            },
            { OsFamilies.Mac, new[] { OsCode.MAC }.ToFrozenSet() },
            {
                OsFamilies.MobileGamingConsole,
                new[] { OsCode.PSP, OsCode.NDS, OsCode.XBX }.ToFrozenSet()
            },
            { OsFamilies.OpenVms, new[] { OsCode.OVS }.ToFrozenSet() },
            {
                OsFamilies.RealtimeOs,
                new[]
                {
                    OsCode.MTK, OsCode.TDX, OsCode.MRE, OsCode.JME, OsCode.REX, OsCode.RXT
                }.ToFrozenSet()
            },
            {
                OsFamilies.OtherMobile,
                new[]
                {
                    OsCode.WOS, OsCode.POS, OsCode.SBA, OsCode.TIZ, OsCode.SMG,
                    OsCode.MAE, OsCode.LUN, OsCode.GEO
                }.ToFrozenSet()
            },
            {
                OsFamilies.Symbian, new[]
                    {
                        OsCode.SYM, OsCode.SYS, OsCode.SY3, OsCode.S60, OsCode.S40
                    }
                    .ToFrozenSet()
            },
            {
                OsFamilies.Unix,
                new[]
                {
                    OsCode.SOS, OsCode.AIX, OsCode.HPX, OsCode.BSD, OsCode.NBS,
                    OsCode.OBS, OsCode.DFB, OsCode.SYL, OsCode.IRI, OsCode.T64,
                    OsCode.INF, OsCode.ELE, OsCode.GNX, OsCode.ULT, OsCode.NWS,
                    OsCode.NXT, OsCode.SBL
                }.ToFrozenSet()
            },
            { OsFamilies.WebTv, new[] { OsCode.WTV }.ToFrozenSet() },
            { OsFamilies.Windows, new[] { OsCode.WIN }.ToFrozenSet() },
            {
                OsFamilies.WindowsMobile,
                new[]
                {
                    OsCode.WPH, OsCode.WMO, OsCode.WCE, OsCode.WRT, OsCode.WIO, OsCode.KIN
                }.ToFrozenSet()
            },
            { OsFamilies.OtherSmartTv, new[] { OsCode.WHS }.ToFrozenSet() }
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Contains a list of mappings from our OS names to known client hint values
    /// </summary>
    private static readonly FrozenDictionary<string, FrozenSet<string>> ClientHintPlatformMapping =
        new Dictionary<string, FrozenSet<string>>
        {
            { OsNames.GnuLinux, new[] { "Linux" }.ToFrozenSet(StringComparer.OrdinalIgnoreCase) },
            { OsNames.Mac, new[] { "MacOS" }.ToFrozenSet(StringComparer.OrdinalIgnoreCase) }
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Operating system families that are known as desktop only
    /// </summary>
    private static readonly FrozenSet<string> DesktopOsFamilies = new[]
    {
        OsNames.AmigaOs, OsFamilies.Ibm, OsFamilies.GnuLinux, OsFamilies.Mac, OsFamilies.Unix, OsFamilies.Windows,
        OsFamilies.BeOs, OsFamilies.ChromeOs,
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    private static readonly FrozenDictionary<string, string> FireOsVersionMapping = new Dictionary<string, string>
    {
        { "11", "8" },
        { "10", "8" },
        { "9", "7" },
        { "7", "6" },
        { "5", "5" },
        { "4.4.3", "4.5.1" },
        { "4.4.2", "4" },
        { "4.2.2", "3" },
        { "4.0.3", "3" },
        { "4.0.2", "3" },
        { "4", "2" },
        { "2", "1" },
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, string> LineageOsVersionMapping = new Dictionary<string, string>()
    {
        { "15", "22" },
        { "14", "21" },
        { "13", "20.0" },
        { "12.1", "19.1" },
        { "12", "19.0" },
        { "11", "18.0" },
        { "10", "17.0" },
        { "9", "16.0" },
        { "8.1.0", "15.1" },
        { "8.0.0", "15.0" },
        { "7.1.2", "14.1" },
        { "7.1.1", "14.1" },
        { "7.0", "14.0" },
        { "6.0.1", "13.0" },
        { "6.0", "13.0" },
        { "5.1.1", "12.1" },
        { "5.0.2", "12.0" },
        { "5.0", "12.0" },
        { "4.4.4", "11.0" },
        { "4.3", "10.2" },
        { "4.2.2", "10.1" },
        { "4.0.4", "9.1.0" }
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<int, string> WindowsMinorVersionMapping =
        new Dictionary<int, string>() { { 1, "7" }, { 2, "8" }, { 3, "8.1" } }.ToFrozenDictionary();


    private string MapPlatformHintToOsName(string platform)
    {
        foreach (var clientHints in ClientHintPlatformMapping)
        {
            if (clientHints.Value.Contains(platform))
            {
                return clientHints.Key;
            }
        }

        return platform;
    }

    /// <summary>
    /// Returns the OS that can be detected from client hints
    /// </summary>
    private bool TryParseOsFromClientHints(ClientHints? clientHints, out OsInfo? osInfo)
    {
        if (clientHints?.Platform is null)
        {
            osInfo = null;
            return false;
        }

        osInfo = new OsInfo
        {
            Name = MapPlatformHintToOsName(clientHints.Platform).CollapseSpaces(),
            Version = clientHints.PlatformVersion
        };

        OsNameMapping.TryGetValue(osInfo.Name, out var code);
        osInfo.Code = code;

        if (osInfo.Name != OsNames.Windows || string.IsNullOrEmpty(osInfo.Version))
        {
            return false;
        }

        var versionParts = osInfo.Version?.Split('.');
        int majorVersion = versionParts?.Length > 0 && int.TryParse(versionParts[0], out var major) ? major : 0;
        int minorVersion = versionParts?.Length > 1 && int.TryParse(versionParts[1], out var minor) ? minor : 0;

        switch (majorVersion)
        {
            case 0 when minorVersion != 0:
                WindowsMinorVersionMapping.TryGetValue(minorVersion, out var version);
                osInfo.Version = version;
                break;
            case > 0 and <= 10:
                osInfo.Version = "10";
                break;
            case > 10:
                osInfo.Version = "11";
                break;
        }

        return osInfo.Version is not null;
    }

    private bool TryParseOsFromUserAgent(string userAgent, out OsInfo? osInfo)
    {


        foreach (var os in OsRegexes)
        {
            /*var match = os.Regex.Match(userAgent);

            if (match.Success)
            {
                break;
            }*/
        }




        throw new NotImplementedException();
    }

    public OsInfo Parse(string userAgent, IDictionary<string, string>? clientHints = null)
    {
        var osInfo = new OsInfo();


        return osInfo;
    }
}
