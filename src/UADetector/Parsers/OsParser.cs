using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Models.Constants;
using UADetector.Models.Enums;
using UADetector.Regexes.Models;
using UADetector.Results;
using UADetector.Utils;

namespace UADetector.Parsers;

public sealed class OsParser : IOsParser
{
    private readonly ParserOptions _parserOptions;
    private const string ResourceName = "Regexes.Resources.oss.yml";

    private static readonly IEnumerable<Os> OsRegexes =
        ParserExtensions.LoadRegexes<Os>(ResourceName, RegexPatternType.UserAgent);

    private static readonly FrozenDictionary<OsCode, string> OsCodeMapping =
        new Dictionary<OsCode, string>
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

    private static readonly FrozenDictionary<string, OsCode> OsNameMapping = OsCodeMapping
        .ToDictionary(e => e.Value, e => e.Key)
        .ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

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

    private static readonly FrozenDictionary<string, string> LineageOsVersionMapping = new Dictionary<string, string>
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
        new Dictionary<int, string> { { 1, "7" }, { 2, "8" }, { 3, "8.1" } }.ToFrozenDictionary();

    private static readonly FrozenSet<string> AndroidApps = new[]
    {
        "com.hisense.odinbrowser", "com.seraphic.openinet.pre", "com.appssppa.idesktoppcbrowser",
        "every.browser.inc"
    }.ToFrozenSet();

    private static readonly FrozenDictionary<string, Regex> PlatformRegexes =
        new Dictionary<string, Regex>
        {
            {
                OsPlatformTypes.Arm,
                ParserExtensions.BuildUserAgentRegex(
                    "arm[ _;)ev]|.*arm$|.*arm64|aarch64|Apple ?TV|Watch ?OS|Watch1,[12]")
            },
            { OsPlatformTypes.LoongArch64, ParserExtensions.BuildUserAgentRegex("loongarch64") },
            { OsPlatformTypes.Mips, ParserExtensions.BuildUserAgentRegex("mips") },
            { OsPlatformTypes.SuperH, ParserExtensions.BuildUserAgentRegex("sh4") },
            { OsPlatformTypes.Sparc64, ParserExtensions.BuildUserAgentRegex("sparc64") },
            {
                OsPlatformTypes.X64,
                ParserExtensions.BuildUserAgentRegex("64-?bit|WOW64|(?:Intel)?x64|WINDOWS_64|win64|.*amd64|.*x86_?64")
            },
            { OsPlatformTypes.X86, ParserExtensions.BuildUserAgentRegex("32bit|win32|(?:i[0-9]|x)86|i86pc") }
        }.ToFrozenDictionary();


    public OsParser(ParserOptions? parserOptions = null)
    {
        _parserOptions = parserOptions ?? new ParserOptions();
    }

    private static bool TryMapPlatformToOsName(string platform, [NotNullWhen((true))] out string? result)
    {
        foreach (var clientHints in ClientHintPlatformMapping)
        {
            if (clientHints.Value.Contains(platform))
            {
                result = clientHints.Key;
                return true;
            }
        }

        result = null;
        return false;
    }

    private static bool TryMapOsNameToOsFamily(string name, [NotNullWhen((true))] out string? result)
    {
        if (OsNameMapping.TryGetValue(name, out var code))
        {
            return TryMapOsCodeToOsFamily(code, out result);
        }

        result = null;
        return false;
    }

    private static bool TryMapOsCodeToOsFamily(OsCode code, [NotNullWhen((true))] out string? result)
    {
        foreach (var osFamily in OsFamilyMapping)
        {
            if (osFamily.Value.Contains(code))
            {
                result = osFamily.Key;
                return true;
            }
        }

        result = null;
        return false;
    }

    private static bool TryGetFireOsVersion(string version, [NotNullWhen((true))] out string? result)
    {
        var index = version.IndexOf('.');

        if (index != -1)
        {
            result = FireOsVersionMapping.TryGetValue(version, out result) ? result :
                FireOsVersionMapping.TryGetValue(version[..index], out result) ? result : null;
        }
        else
        {
            result = null;
        }

        return result is not null;
    }

    private static bool TryGetLineageOsVersion(string version, [NotNullWhen((true))] out string? result)
    {
        var index = version.IndexOf('.');

        if (index != -1)
        {
            result = LineageOsVersionMapping.TryGetValue(version, out result) ? result :
                LineageOsVersionMapping.TryGetValue(version[..index], out result) ? result : null;
        }
        else
        {
            result = null;
        }

        return result is not null;
    }

    private static bool TryParsePlatform(string userAgent, ClientHints? clientHints, [NotNullWhen(true)] out string? result)
    {
        result = null;

        if (clientHints?.Architecture is not null)
        {
            var architecture = clientHints.Architecture.ToLower();

            if (architecture.Contains("arm"))
            {
                result = OsPlatformTypes.Arm;
            }
            else if (architecture.Contains("loongarch64"))
            {
                result = OsPlatformTypes.LoongArch64;
            }
            else if (architecture.Contains("mips"))
            {
                result = OsPlatformTypes.Mips;
            }
            else if (architecture.Contains("sh4"))
            {
                result = OsPlatformTypes.SuperH;
            }
            else if (architecture.Contains("sparc64"))
            {
                result = OsPlatformTypes.Sparc64;
            }
            else if (architecture.Contains("x64") || (architecture.Contains("x86") && clientHints.Bitness == "64"))
            {
                result = OsPlatformTypes.X64;
            }
            else if (architecture.Contains("x86"))
            {
                result = OsPlatformTypes.X86;
            }

            if (result is not null)
            {
                return true;
            }
        }

        if (PlatformRegexes.TryGetValue(OsPlatformTypes.Arm, out var regex) && regex.IsMatch(userAgent))
        {
            result = OsPlatformTypes.Arm;
        }
        else if (PlatformRegexes.TryGetValue(OsPlatformTypes.LoongArch64, out regex) && regex.IsMatch(userAgent))
        {
            result = OsPlatformTypes.LoongArch64;
        }
        else if (PlatformRegexes.TryGetValue(OsPlatformTypes.Mips, out regex) && regex.IsMatch(userAgent))
        {
            result = OsPlatformTypes.Mips;
        }
        else if (PlatformRegexes.TryGetValue(OsPlatformTypes.SuperH, out regex) && regex.IsMatch(userAgent))
        {
            result = OsPlatformTypes.SuperH;
        }
        else if (PlatformRegexes.TryGetValue(OsPlatformTypes.Sparc64, out regex) && regex.IsMatch(userAgent))
        {
            result = OsPlatformTypes.Sparc64;
        }
        else if (PlatformRegexes.TryGetValue(OsPlatformTypes.X64, out regex) && regex.IsMatch(userAgent))
        {
            result = OsPlatformTypes.X64;
        }
        else if (PlatformRegexes.TryGetValue(OsPlatformTypes.X86, out regex) && regex.IsMatch(userAgent))
        {
            result = OsPlatformTypes.X86;
        }

        return result is not null;
    }

    private bool TryParseOsFromClientHints(ClientHints clientHints, [NotNullWhen(true)] out BaseOsInfo? result)
    {
        if (clientHints.Platform is null)
        {
            result = null;
            return false;
        }

        OsCode? code = null;

        if (TryMapPlatformToOsName(clientHints.Platform, out var name))
        {
            name.CollapseSpaces();

            if (OsNameMapping.TryGetValue(name, out var osCode))
            {
                code = osCode;
            }
        }

        string? version = clientHints.PlatformVersion;

        if (name == OsNames.Windows && !string.IsNullOrEmpty(version))
        {
            var versionParts = version?.Split('.');
            int majorVersion = versionParts?.Length > 0 && int.TryParse(versionParts[0], out var major) ? major : 0;
            int minorVersion = versionParts?.Length > 1 && int.TryParse(versionParts[1], out var minor) ? minor : 0;

            switch (majorVersion)
            {
                case 0 when minorVersion != 0:
                    WindowsMinorVersionMapping.TryGetValue(minorVersion, out version);
                    break;
                case > 0 and <= 10:
                    version = "10";
                    break;
                case > 10:
                    version = "11";
                    break;
            }
        }

        // On Windows, version 0.0.0 can represent either 7, 8, or 8.1, so the value is set to null.
        if (name != OsNames.Windows && version != "0.0.0" && !int.TryParse(version, out _))
        {
            version = null;
        }

        result = new BaseOsInfo
        {
            Name = name,
            Code = code,
            Version = version,
        };

        return true;
    }

    private bool TryParseOsFromUserAgent(string userAgent, [NotNullWhen(true)] out BaseOsInfo? result)
    {
        Match? match = null;
        Os? os = null;

        foreach (var osRegex in OsRegexes)
        {
            match = osRegex.Regex.Match(userAgent);

            if (match.Success)
            {
                os = osRegex;
                break;
            }
        }

        if (os is null || match is null || !match.Success)
        {
            result = null;
            return false;
        }

        string? name = ParserExtensions.FormatWithMatch(os.Name, match);
        string? version = null;
        OsCode? code = null;

        if (name is not null && OsNameMapping.TryGetValue(name, out var osCode))
        {
            code = osCode;
        }

        if (!string.IsNullOrEmpty(os.Version))
        {
            version =
                ParserExtensions.FormatVersionWithMatch(os.Version, match, _parserOptions.VersionTruncation);
        }

        if (os.Versions?.Count > 0)
        {
            foreach (var versionRegex in os.Versions)
            {
                match = versionRegex.Regex.Match(userAgent);

                if (match.Success)
                {
                    version = ParserExtensions.FormatVersionWithMatch(versionRegex.Version, match,
                        _parserOptions.VersionTruncation);
                    break;
                }
            }
        }

        result = new BaseOsInfo { Name = name, Code = code, Version = version, };
        return true;
    }

    public bool TryParse(string userAgent, [NotNullWhen(true)] out OsInfo? result)
    {
        return TryParse(userAgent, null, out result);
    }

    public bool TryParse(string userAgent, ClientHints? clientHints, [NotNullWhen(true)] out OsInfo? result)
    {
        if (clientHints is not null &&
            ParserExtensions.TryRestoreUserAgent(userAgent, clientHints, out var restoredUserAgent))
        {
            userAgent = restoredUserAgent;
        }

        string? name, version;
        OsCode? code;

        if (TryParseOsFromUserAgent(userAgent, out var osFromUserAgent) && clientHints is not null &&
            TryParseOsFromClientHints(clientHints, out var osFromClientHints) &&
            osFromClientHints.Name is not null)
        {
            name = osFromClientHints.Name;
            version = osFromClientHints.Version;
            code = osFromClientHints.Code;
            string? osFamilyFromUserAgent = null;

            // Use the version from the user agent if none was provided in the client hints, 
            // but the OS family from the user agent matches.
            if (string.IsNullOrEmpty(osFromClientHints.Version) && osFromUserAgent.Name is not null &&
                TryMapOsNameToOsFamily(osFromClientHints.Name, out var osFamilyFromClientHints) &&
                TryMapOsNameToOsFamily(osFromUserAgent.Name, out osFamilyFromUserAgent) &&
                osFamilyFromClientHints == osFamilyFromUserAgent)
            {
                version = osFromUserAgent.Version;
            }

            // On Windows, version 0.0.0 can represent either 7, 8, or 8.1
            if (name == OsNames.Windows && version == "0.0.0")
            {
                version = osFromUserAgent.Version == "10" ? null : osFromUserAgent.Version;
            }

            // If the OS name detected from client hints matches the OS family from user agent
            // but the os name is another, we use the one from user agent, as it might be more detailed
            if (osFamilyFromUserAgent == name && osFromUserAgent.Name != name)
            {
                name = osFromUserAgent.Name;

                switch (name)
                {
                    case OsNames.LeafOs or OsNames.HarmonyOs:
                        version = null;
                        break;
                    case OsNames.PicoOs:
                        version = osFromUserAgent.Version;
                        break;
                    case OsNames.FireOs when version is not null:
                        {
                            TryGetFireOsVersion(version, out version);
                            break;
                        }
                }
            }

            switch (name)
            {
                // Chrome OS is in some cases reported as Linux in client hints, we fix this only if the version matches
                case OsNames.GnuLinux when osFromUserAgent.Name == OsNames.ChromeOs &&
                                           osFromClientHints.Version == osFromUserAgent.Version:
                    name = osFromUserAgent.Name;
                    code = osFromUserAgent.Code;
                    break;
                // Chrome OS is in some cases reported as Android in client hints
                case OsNames.Android when osFromUserAgent.Name == OsNames.ChromeOs:
                    name = osFromUserAgent.Name;
                    code = osFromUserAgent.Code;
                    version = null;
                    break;
                // Meta Horizon is reported as Linux in client hints
                case OsNames.GnuLinux when osFromUserAgent.Name == OsNames.MetaHorizon:
                    name = osFromUserAgent.Name;
                    code = osFromUserAgent.Code;
                    break;
            }
        }
        else if (osFromUserAgent?.Name is not null)
        {
            name = osFromUserAgent.Name;
            code = osFromUserAgent.Code;
            version = osFromUserAgent.Version;
        }
        else
        {
            result = null;
            return false;
        }

        TryParsePlatform(userAgent, clientHints, out var platform);

        string? family = null;

        if (code is not null)
        {
            TryMapOsCodeToOsFamily(code.Value, out family);
        }

        if (clientHints?.App is not null)
        {
            if (name != OsNames.Android && AndroidApps.Contains(clientHints.App))
            {
                name = OsNames.Android;
                family = OsFamilies.Android;
                code = OsCode.ADR;
                version = null;
            }
            else if (name != OsNames.LineageOs && clientHints.App == "org.lineageos.jelly")
            {
                name = OsNames.LineageOs;
                family = OsFamilies.Android;
                code = OsCode.LEN;

                if (version is not null)
                {
                    TryGetLineageOsVersion(version, out version);
                }
            }
            else if (name != OsNames.FireOs && clientHints.App == "org.mozilla.tv.firefox")
            {
                name = OsNames.FireOs;
                family = OsFamilies.Android;
                code = OsCode.FIR;

                if (version is not null)
                {
                    TryGetFireOsVersion(version, out version);
                }
            }
        }

        result = new OsInfo
        {
            Name = name ?? string.Empty,
            Code = code ?? default(OsCode),
            Version = version,
            Platform = platform,
            Family = family
        };

        return true;
    }

    private sealed class BaseOsInfo
    {
        public string? Name { get; init; }
        public OsCode? Code { get; init; }
        public string? Version { get; init; }
    }
}
