using System.Collections.Frozen;

using UADetector.Models;
using UADetector.Regexes.Models;

namespace UADetector.Parsers;

public class OperatingSystemParser
{
    private const string ResourceName = "Regexes.Resources.operating_systems.yml";

    private static readonly IEnumerable<OperatingSystemRegex> Regexes =
        ParserExtensions.LoadRegexes<OperatingSystemRegex>(ResourceName);

    private static readonly FrozenDictionary<OsShortName, string> OsShortNameToFullNameMapping =
        new Dictionary<OsShortName, string>
        {
            { OsShortName.AIX, OsNames.Aix },
            { OsShortName.AND, OsNames.Android },
            { OsShortName.ADR, OsNames.AndroidTv },
            { OsShortName.ALP, OsNames.AlpineLinux },
            { OsShortName.AMZ, OsNames.AmazonLinux },
            { OsShortName.AMG, OsNames.AmigaOs },
            { OsShortName.ARM, OsNames.ArmadilloOs },
            { OsShortName.ARO, OsNames.Aros },
            { OsShortName.ATV, OsNames.TvOs },
            { OsShortName.ARL, OsNames.ArchLinux },
            { OsShortName.AOS, OsNames.AosCos },
            { OsShortName.ASP, OsNames.AspLinux },
            { OsShortName.AZU, OsNames.AzureLinux },
            { OsShortName.BTR, OsNames.BackTrack },
            { OsShortName.SBA, OsNames.Bada },
            { OsShortName.BYI, OsNames.BaiduYi },
            { OsShortName.BEO, OsNames.BeOs },
            { OsShortName.BLB, OsNames.BlackBerryOs },
            { OsShortName.QNX, OsNames.BlackBerryTabletOs },
            { OsShortName.PAN, OsNames.BlackPantherOs },
            { OsShortName.BOS, OsNames.BlissOs },
            { OsShortName.BMP, OsNames.Brew },
            { OsShortName.BSN, OsNames.BrightSignOs },
            { OsShortName.CAI, OsNames.CaixaMagica },
            { OsShortName.CES, OsNames.CentOs },
            { OsShortName.CST, OsNames.CentOsStream },
            { OsShortName.CLO, OsNames.ClearLinuxOs },
            { OsShortName.CLR, OsNames.ClearOsMobile },
            { OsShortName.COS, OsNames.ChromeOs },
            { OsShortName.CRS, OsNames.ChromiumOs },
            { OsShortName.CHN, OsNames.ChinaOs },
            { OsShortName.COL, OsNames.CoolitaOs },
            { OsShortName.CYN, OsNames.CyanogenMod },
            { OsShortName.DEB, OsNames.Debian },
            { OsShortName.DEE, OsNames.Deepin },
            { OsShortName.DFB, OsNames.DragonFly },
            { OsShortName.DVK, OsNames.DvkBuntu },
            { OsShortName.ELE, OsNames.ElectroBsd },
            { OsShortName.EUL, OsNames.EulerOs },
            { OsShortName.FED, OsNames.Fedora },
            { OsShortName.FEN, OsNames.Fenix },
            { OsShortName.FOS, OsNames.FirefoxOs },
            { OsShortName.FIR, OsNames.FireOs },
            { OsShortName.FOR, OsNames.ForesightLinux },
            { OsShortName.FRE, OsNames.Freebox },
            { OsShortName.BSD, OsNames.FreeBsd },
            { OsShortName.FRI, OsNames.FritzOs },
            { OsShortName.FYD, OsNames.FydeOs },
            { OsShortName.FUC, OsNames.Fuchsia },
            { OsShortName.GNT, OsNames.Gentoo },
            { OsShortName.GNX, OsNames.Genix },
            { OsShortName.GEO, OsNames.Geos },
            { OsShortName.GNS, OsNames.GNewSense },
            { OsShortName.GRI, OsNames.GridOs },
            { OsShortName.GTV, OsNames.GoogleTv },
            { OsShortName.HPX, OsNames.HpUx },
            { OsShortName.HAI, OsNames.HaikuOs },
            { OsShortName.IPA, OsNames.IPadOs },
            { OsShortName.HAR, OsNames.HarmonyOs },
            { OsShortName.HAS, OsNames.HasCodingOs },
            { OsShortName.HEL, OsNames.HelixOs },
            { OsShortName.IRI, OsNames.Irix },
            { OsShortName.INF, OsNames.Inferno },
            { OsShortName.JME, OsNames.JavaMe },
            { OsShortName.JOL, OsNames.JoliOs },
            { OsShortName.KOS, OsNames.KaiOs },
            { OsShortName.KAL, OsNames.Kali },
            { OsShortName.KAN, OsNames.Kanotix },
            { OsShortName.KIN, OsNames.Kinos },
            { OsShortName.KNO, OsNames.Knoppix },
            { OsShortName.KTV, OsNames.KreaTv },
            { OsShortName.KBT, OsNames.Kubuntu },
            { OsShortName.LIN, OsNames.GnuLinux },
            { OsShortName.LEA, OsNames.LeafOs },
            { OsShortName.LND, OsNames.LindowsOs },
            { OsShortName.LNS, OsNames.Linspire },
            { OsShortName.LEN, OsNames.LineageOs },
            { OsShortName.LIR, OsNames.LiriOs },
            { OsShortName.LOO, OsNames.Loongnix },
            { OsShortName.LBT, OsNames.Lubuntu },
            { OsShortName.LOS, OsNames.LuminOs },
            { OsShortName.LUN, OsNames.LuneOs },
            { OsShortName.VLN, OsNames.VectorLinux },
            { OsShortName.MAC, OsNames.Mac },
            { OsShortName.MAE, OsNames.Maemo },
            { OsShortName.MAG, OsNames.Mageia },
            { OsShortName.MDR, OsNames.Mandriva },
            { OsShortName.SMG, OsNames.MeeGo },
            { OsShortName.MET, OsNames.MetaHorizon },
            { OsShortName.MCD, OsNames.MocorDroid },
            { OsShortName.MON, OsNames.MoonOs },
            { OsShortName.EZX, OsNames.MotorolaEzx },
            { OsShortName.MIN, OsNames.Mint },
            { OsShortName.MLD, OsNames.MildWild },
            { OsShortName.MOR, OsNames.MorphOs },
            { OsShortName.NBS, OsNames.NetBsd },
            { OsShortName.MTK, OsNames.MtkNucleus },
            { OsShortName.MRE, OsNames.Mre },
            { OsShortName.NXT, OsNames.NeXtStep },
            { OsShortName.NWS, OsNames.NewsOs },
            { OsShortName.WII, OsNames.Nintendo },
            { OsShortName.NDS, OsNames.NintendoMobile },
            { OsShortName.NOV, OsNames.Nova },
            { OsShortName.OS2, OsNames.Os2 },
            { OsShortName.T64, OsNames.Osf1 },
            { OsShortName.OBS, OsNames.OpenBsd },
            { OsShortName.OVS, OsNames.OpenVms },
            { OsShortName.OVZ, OsNames.OpenVz },
            { OsShortName.OWR, OsNames.OpenWrt },
            { OsShortName.OTV, OsNames.OperaTv },
            { OsShortName.ORA, OsNames.OracleLinux },
            { OsShortName.ORD, OsNames.Ordissimo },
            { OsShortName.PAR, OsNames.Pardus },
            { OsShortName.PCL, OsNames.PcLinuxOs },
            { OsShortName.PIC, OsNames.PicoOs },
            { OsShortName.PLA, OsNames.PlasmaMobile },
            { OsShortName.PSP, OsNames.PlayStationPortable },
            { OsShortName.PS3, OsNames.PlayStation },
            { OsShortName.PVE, OsNames.ProxmoxVe },
            { OsShortName.PUF, OsNames.PuffinOs },
            { OsShortName.PUR, OsNames.PureOs },
            { OsShortName.QTP, OsNames.Qtopia },
            { OsShortName.PIO, OsNames.RaspberryPiOs },
            { OsShortName.RAS, OsNames.Raspbian },
            { OsShortName.RHT, OsNames.RedHat },
            { OsShortName.RST, OsNames.RedStar },
            { OsShortName.RED, OsNames.RedOs },
            { OsShortName.REV, OsNames.RevengeOs },
            { OsShortName.RIS, OsNames.RisingOs },
            { OsShortName.ROS, OsNames.RiscOs },
            { OsShortName.ROC, OsNames.RockyLinux },
            { OsShortName.ROK, OsNames.RokuOs },
            { OsShortName.RSO, OsNames.Rosa },
            { OsShortName.ROU, OsNames.RouterOs },
            { OsShortName.REM, OsNames.RemixOs },
            { OsShortName.RRS, OsNames.ResurrectionRemixOs },
            { OsShortName.REX, OsNames.Rex },
            { OsShortName.RZD, OsNames.RazoDroid },
            { OsShortName.RXT, OsNames.RtosNext },
            { OsShortName.SAB, OsNames.Sabayon },
            { OsShortName.SSE, OsNames.Suse },
            { OsShortName.SAF, OsNames.SailfishOs },
            { OsShortName.SCI, OsNames.ScientificLinux },
            { OsShortName.SEE, OsNames.SeewoOs },
            { OsShortName.SER, OsNames.SerenityOs },
            { OsShortName.SIR, OsNames.SirinOs },
            { OsShortName.SLW, OsNames.Slackware },
            { OsShortName.SOS, OsNames.Solaris },
            { OsShortName.SBL, OsNames.StarBladeOs },
            { OsShortName.SYL, OsNames.Syllable },
            { OsShortName.SYM, OsNames.Symbian },
            { OsShortName.SYS, OsNames.SymbianOs },
            { OsShortName.S40, OsNames.SymbianOsSeries40 },
            { OsShortName.S60, OsNames.SymbianOsSeries60 },
            { OsShortName.SY3, OsNames.Symbian3 },
            { OsShortName.TEN, OsNames.TencentOs },
            { OsShortName.TDX, OsNames.ThreadX },
            { OsShortName.TIZ, OsNames.Tizen },
            { OsShortName.TIV, OsNames.TiVoOs },
            { OsShortName.TOS, OsNames.TmaxOs },
            { OsShortName.TUR, OsNames.Turbolinux },
            { OsShortName.UBT, OsNames.Ubuntu },
            { OsShortName.ULT, OsNames.Ultrix },
            { OsShortName.UOS, OsNames.Uos },
            { OsShortName.VID, OsNames.Vidae },
            { OsShortName.VIZ, OsNames.ViziOs },
            { OsShortName.WAS, OsNames.WatchOs },
            { OsShortName.WER, OsNames.WearOs },
            { OsShortName.WTV, OsNames.WebTv },
            { OsShortName.WHS, OsNames.WhaleOs },
            { OsShortName.WIN, OsNames.Windows },
            { OsShortName.WCE, OsNames.WindowsCe },
            { OsShortName.WIO, OsNames.WindowsIoT },
            { OsShortName.WMO, OsNames.WindowsMobile },
            { OsShortName.WPH, OsNames.WindowsPhone },
            { OsShortName.WRT, OsNames.WindowsRt },
            { OsShortName.WPO, OsNames.WoPhone },
            { OsShortName.XBX, OsNames.Xbox },
            { OsShortName.XBT, OsNames.Xubuntu },
            { OsShortName.YNS, OsNames.YunOs },
            { OsShortName.ZEN, OsNames.Zenwalk },
            { OsShortName.ZOR, OsNames.ZorinOs },
            { OsShortName.IOS, OsNames.IOs },
            { OsShortName.POS, OsNames.PalmOs },
            { OsShortName.WEB, OsNames.Webian },
            { OsShortName.WOS, OsNames.WebOs },
        }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, OsShortName> OsFullNameToShortNameMapping =
        OsShortNameToFullNameMapping.ToDictionary(e => e.Value, e => e.Key)
            .ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    
    private static readonly FrozenDictionary<string, FrozenSet<OsShortName>> OsFamilyToShortNameMapping =
        new Dictionary<string, FrozenSet<OsShortName>>
        {
            {
                OsFamilies.Android,
                new[]
                {
                    OsShortName.AND, OsShortName.CYN, OsShortName.FIR, OsShortName.REM, OsShortName.RZD,
                    OsShortName.MLD, OsShortName.MCD, OsShortName.YNS, OsShortName.GRI, OsShortName.HAR,
                    OsShortName.ADR, OsShortName.CLR, OsShortName.BOS, OsShortName.REV, OsShortName.LEN,
                    OsShortName.SIR, OsShortName.RRS, OsShortName.WER, OsShortName.PIC, OsShortName.ARM,
                    OsShortName.HEL, OsShortName.BYI, OsShortName.RIS, OsShortName.PUF, OsShortName.LEA, OsShortName.MET
                }.ToFrozenSet()
            },
            { OsFamilies.AmigaOs, new[] { OsShortName.AMG, OsShortName.MOR, OsShortName.ARO }.ToFrozenSet() },
            { OsFamilies.BlackBerry, new[] { OsShortName.BLB, OsShortName.QNX }.ToFrozenSet() },
            { OsFamilies.Brew, new[] { OsShortName.BMP }.ToFrozenSet() },
            { OsFamilies.BeOs, new[] { OsShortName.BEO, OsShortName.HAI }.ToFrozenSet() },
            {
                OsFamilies.ChromeOs,
                new[] { OsShortName.COS, OsShortName.CRS, OsShortName.FYD, OsShortName.SEE }.ToFrozenSet()
            },
            { OsFamilies.FirefoxOs, new[] { OsShortName.FOS, OsShortName.KOS }.ToFrozenSet() },
            { OsFamilies.GamingConsole, new[] { OsShortName.WII, OsShortName.PS3 }.ToFrozenSet() },
            { OsFamilies.GoogleTv, new[] { OsShortName.GTV }.ToFrozenSet() },
            { OsFamilies.Ibm, new[] { OsShortName.OS2 }.ToFrozenSet() },
            {
                OsFamilies.Ios,
                new[] { OsShortName.IOS, OsShortName.ATV, OsShortName.WAS, OsShortName.IPA }.ToFrozenSet()
            },
            { OsFamilies.RiscOs, new[] { OsShortName.ROS }.ToFrozenSet() },
            {
                OsFamilies.GnuLinux,
                new[]
                {
                    OsShortName.LIN, OsShortName.ARL, OsShortName.DEB, OsShortName.KNO, OsShortName.MIN,
                    OsShortName.UBT, OsShortName.KBT, OsShortName.XBT, OsShortName.LBT, OsShortName.FED,
                    OsShortName.RHT, OsShortName.VLN, OsShortName.MDR, OsShortName.GNT, OsShortName.SAB,
                    OsShortName.SLW, OsShortName.SSE, OsShortName.CES, OsShortName.BTR, OsShortName.SAF,
                    OsShortName.ORD, OsShortName.TOS, OsShortName.RSO, OsShortName.DEE, OsShortName.FRE,
                    OsShortName.MAG, OsShortName.FEN, OsShortName.CAI, OsShortName.PCL, OsShortName.HAS,
                    OsShortName.LOS, OsShortName.DVK, OsShortName.ROK, OsShortName.OWR, OsShortName.OTV,
                    OsShortName.KTV, OsShortName.PUR, OsShortName.PLA, OsShortName.FUC, OsShortName.PAR,
                    OsShortName.FOR, OsShortName.MON, OsShortName.KAN, OsShortName.ZEN, OsShortName.LND,
                    OsShortName.LNS, OsShortName.CHN, OsShortName.AMZ, OsShortName.TEN, OsShortName.CST,
                    OsShortName.NOV, OsShortName.ROU, OsShortName.ZOR, OsShortName.RED, OsShortName.KAL,
                    OsShortName.ORA, OsShortName.VID, OsShortName.TIV, OsShortName.BSN, OsShortName.RAS,
                    OsShortName.UOS, OsShortName.PIO, OsShortName.FRI, OsShortName.LIR, OsShortName.WEB,
                    OsShortName.SER, OsShortName.ASP, OsShortName.AOS, OsShortName.LOO, OsShortName.EUL,
                    OsShortName.SCI, OsShortName.ALP, OsShortName.CLO, OsShortName.ROC, OsShortName.OVZ,
                    OsShortName.PVE, OsShortName.RST, OsShortName.EZX, OsShortName.GNS, OsShortName.JOL,
                    OsShortName.TUR, OsShortName.QTP, OsShortName.WPO, OsShortName.PAN, OsShortName.VIZ,
                    OsShortName.AZU, OsShortName.COL
                }.ToFrozenSet()
            },
            { OsFamilies.Mac, new[] { OsShortName.MAC }.ToFrozenSet() },
            {
                OsFamilies.MobileGamingConsole,
                new[] { OsShortName.PSP, OsShortName.NDS, OsShortName.XBX }.ToFrozenSet()
            },
            { OsFamilies.OpenVms, new[] { OsShortName.OVS }.ToFrozenSet() },
            {
                OsFamilies.RealtimeOs,
                new[]
                {
                    OsShortName.MTK, OsShortName.TDX, OsShortName.MRE, OsShortName.JME, OsShortName.REX, OsShortName.RXT
                }.ToFrozenSet()
            },
            {
                OsFamilies.OtherMobile,
                new[]
                {
                    OsShortName.WOS, OsShortName.POS, OsShortName.SBA, OsShortName.TIZ, OsShortName.SMG,
                    OsShortName.MAE, OsShortName.LUN, OsShortName.GEO
                }.ToFrozenSet()
            },
            {
                OsFamilies.Symbian, new[]
                    {
                        OsShortName.SYM, OsShortName.SYS, OsShortName.SY3, OsShortName.S60, OsShortName.S40
                    }
                    .ToFrozenSet()
            },
            {
                OsFamilies.Unix,
                new[]
                {
                    OsShortName.SOS, OsShortName.AIX, OsShortName.HPX, OsShortName.BSD, OsShortName.NBS,
                    OsShortName.OBS, OsShortName.DFB, OsShortName.SYL, OsShortName.IRI, OsShortName.T64,
                    OsShortName.INF, OsShortName.ELE, OsShortName.GNX, OsShortName.ULT, OsShortName.NWS,
                    OsShortName.NXT, OsShortName.SBL
                }.ToFrozenSet()
            },
            { OsFamilies.WebTv, new[] { OsShortName.WTV }.ToFrozenSet() },
            { OsFamilies.Windows, new[] { OsShortName.WIN }.ToFrozenSet() },
            {
                OsFamilies.WindowsMobile,
                new[]
                {
                    OsShortName.WPH, OsShortName.WMO, OsShortName.WCE, OsShortName.WRT, OsShortName.WIO, OsShortName.KIN
                }.ToFrozenSet()
            },
            { OsFamilies.OtherSmartTv, new[] { OsShortName.WHS }.ToFrozenSet() }
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Contains a list of mappings from OS names we use to known client hint values
    /// </summary>
    private static readonly FrozenDictionary<string, FrozenSet<string>> ClientHintMapping =
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
        OsFamilies.BeOs, OsFamilies.ChromeOs, OsFamilies.ChromiumOs,
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
}
