using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Abstractions.Constants;
using UaDetector.Abstractions.Enums;
using UaDetector.Abstractions.Models;
using UaDetector.Attributes;
using UaDetector.Models;
using UaDetector.Registries;
using UaDetector.Utilities;

namespace UaDetector.Parsers;

public sealed partial class OsParser : IOsParser
{
    [RegexSource("Resources/operating_systems.json")]
    internal static partial IReadOnlyList<Os> OperatingSystems { get; }

    private const string CacheKeyPrefix = "os";
    private readonly IUaDetectorCache? _cache;
    private readonly UaDetectorOptions _uaDetectorOptions;
    private readonly BotParser _botParser;

    internal static readonly FrozenDictionary<string, FrozenSet<OsCode>> OsFamilyMappings;
    internal static readonly FrozenSet<string> DesktopOsFamilies;
    private static readonly FrozenDictionary<string, FrozenSet<string>> ClientHintPlatformMappings;
    private static readonly FrozenDictionary<string, string> FireOsVersionMappings;
    private static readonly FrozenDictionary<string, string> LineageOsVersionMappings;
    private static readonly FrozenDictionary<int, string> WindowsMinorVersionMappings;
    private static readonly FrozenSet<string> AndroidApps;
    private static readonly FrozenDictionary<string, Regex> CpuArchitectureRegexes;
    private static readonly IReadOnlyList<string> ProcessorArchitectures;

    static OsParser()
    {
        OsFamilyMappings = new Dictionary<string, FrozenSet<OsCode>>
        {
            {
                OsFamilies.Android,
                new[]
                {
                    OsCode.Android,
                    OsCode.CyanogenMod,
                    OsCode.FireOs,
                    OsCode.RemixOs,
                    OsCode.RazoDroid,
                    OsCode.MildWild,
                    OsCode.MocorDroid,
                    OsCode.YunOs,
                    OsCode.GridOs,
                    OsCode.HarmonyOs,
                    OsCode.AndroidTv,
                    OsCode.ClearOsMobile,
                    OsCode.BlissOs,
                    OsCode.RevengeOs,
                    OsCode.LineageOs,
                    OsCode.SirinOs,
                    OsCode.ResurrectionRemixOs,
                    OsCode.WearOs,
                    OsCode.PicoOs,
                    OsCode.ArmadilloOs,
                    OsCode.HelixOs,
                    OsCode.BaiduYi,
                    OsCode.RisingOs,
                    OsCode.PuffinOs,
                    OsCode.LeafOs,
                    OsCode.MetaHorizon,
                    OsCode.OpenHarmony,
                    OsCode.SmartisanOs
                }.ToFrozenSet()
            },
            {
                OsFamilies.AmigaOs,
                new[] { OsCode.AmigaOs, OsCode.MorphOs, OsCode.Aros }.ToFrozenSet()
            },
            {
                OsFamilies.BlackBerry,
                new[] { OsCode.BlackBerryOs, OsCode.BlackBerryTabletOs }.ToFrozenSet()
            },
            { OsFamilies.Brew, new[] { OsCode.Brew }.ToFrozenSet() },
            { OsFamilies.BeOs, new[] { OsCode.BeOs, OsCode.HaikuOs }.ToFrozenSet() },
            {
                OsFamilies.ChromeOs,
                new[]
                {
                    OsCode.ChromeOs,
                    OsCode.ChromiumOs,
                    OsCode.FydeOs,
                    OsCode.SeewoOs,
                }.ToFrozenSet()
            },
            { OsFamilies.FirefoxOs, new[] { OsCode.FirefoxOs, OsCode.KaiOs }.ToFrozenSet() },
            {
                OsFamilies.GamingConsole,
                new[] { OsCode.Nintendo, OsCode.PlayStation }.ToFrozenSet()
            },
            { OsFamilies.GoogleTv, new[] { OsCode.GoogleTv }.ToFrozenSet() },
            { OsFamilies.Ibm, new[] { OsCode.Os2 }.ToFrozenSet() },
            {
                OsFamilies.Ios,
                new[] { OsCode.IOs, OsCode.TvOs, OsCode.WatchOs, OsCode.IPadOs }.ToFrozenSet()
            },
            { OsFamilies.RiscOs, new[] { OsCode.RiscOs }.ToFrozenSet() },
            {
                OsFamilies.GnuLinux,
                new[]
                {
                    OsCode.GnuLinux,
                    OsCode.ArchLinux,
                    OsCode.Debian,
                    OsCode.Knoppix,
                    OsCode.Mint,
                    OsCode.Ubuntu,
                    OsCode.Kubuntu,
                    OsCode.Xubuntu,
                    OsCode.Lubuntu,
                    OsCode.Fedora,
                    OsCode.RedHat,
                    OsCode.VectorLinux,
                    OsCode.Mandriva,
                    OsCode.Gentoo,
                    OsCode.Sabayon,
                    OsCode.Slackware,
                    OsCode.Suse,
                    OsCode.CentOs,
                    OsCode.BackTrack,
                    OsCode.SailfishOs,
                    OsCode.Ordissimo,
                    OsCode.TmaxOs,
                    OsCode.Rosa,
                    OsCode.Deepin,
                    OsCode.Freebox,
                    OsCode.Mageia,
                    OsCode.Fenix,
                    OsCode.CaixaMagica,
                    OsCode.PcLinuxOs,
                    OsCode.HasCodingOs,
                    OsCode.LuminOs,
                    OsCode.DvkBuntu,
                    OsCode.RokuOs,
                    OsCode.OpenWrt,
                    OsCode.OperaTv,
                    OsCode.KreaTv,
                    OsCode.PureOs,
                    OsCode.PlasmaMobile,
                    OsCode.Fuchsia,
                    OsCode.Pardus,
                    OsCode.ForesightLinux,
                    OsCode.MoonOs,
                    OsCode.Kanotix,
                    OsCode.Zenwalk,
                    OsCode.LindowsOs,
                    OsCode.Linspire,
                    OsCode.ChinaOs,
                    OsCode.AmazonLinux,
                    OsCode.TencentOs,
                    OsCode.CentOsStream,
                    OsCode.Nova,
                    OsCode.RouterOs,
                    OsCode.ZorinOs,
                    OsCode.RedOs,
                    OsCode.Kali,
                    OsCode.OracleLinux,
                    OsCode.Vidaa,
                    OsCode.TiVoOs,
                    OsCode.BrightSignOs,
                    OsCode.Raspbian,
                    OsCode.Uos,
                    OsCode.RaspberryPiOs,
                    OsCode.FritzOs,
                    OsCode.LiriOs,
                    OsCode.Webian,
                    OsCode.SerenityOs,
                    OsCode.AspLinux,
                    OsCode.AoscOs,
                    OsCode.Loongnix,
                    OsCode.EulerOs,
                    OsCode.ScientificLinux,
                    OsCode.AlpineLinux,
                    OsCode.ClearLinuxOs,
                    OsCode.RockyLinux,
                    OsCode.OpenVz,
                    OsCode.ProxmoxVe,
                    OsCode.RedStar,
                    OsCode.MotorolaEzx,
                    OsCode.GNewSense,
                    OsCode.JoliOs,
                    OsCode.Turbolinux,
                    OsCode.Qtopia,
                    OsCode.WoPhone,
                    OsCode.BlackPantherOs,
                    OsCode.ViziOs,
                    OsCode.AzureLinux,
                    OsCode.CoolitaOs,
                }.ToFrozenSet()
            },
            { OsFamilies.Mac, new[] { OsCode.Mac }.ToFrozenSet() },
            {
                OsFamilies.MobileGamingConsole,
                new[]
                {
                    OsCode.PlayStationPortable,
                    OsCode.NintendoMobile,
                    OsCode.Xbox,
                }.ToFrozenSet()
            },
            { OsFamilies.OpenVms, new[] { OsCode.OpenVms }.ToFrozenSet() },
            {
                OsFamilies.RealtimeOs,
                new[]
                {
                    OsCode.MtkNucleus,
                    OsCode.ThreadX,
                    OsCode.Mre,
                    OsCode.JavaMe,
                    OsCode.Rex,
                    OsCode.RtosAndNext,
                    OsCode.KolibriOs,
                }.ToFrozenSet()
            },
            {
                OsFamilies.OtherMobile,
                new[]
                {
                    OsCode.WebOs,
                    OsCode.PalmOs,
                    OsCode.Bada,
                    OsCode.Tizen,
                    OsCode.MeeGo,
                    OsCode.Maemo,
                    OsCode.LuneOs,
                    OsCode.Geos,
                }.ToFrozenSet()
            },
            {
                OsFamilies.Symbian,
                new[]
                {
                    OsCode.Symbian,
                    OsCode.SymbianOs,
                    OsCode.Symbian3,
                    OsCode.SymbianOsSeries40,
                    OsCode.SymbianOsSeries60,
                }.ToFrozenSet()
            },
            {
                OsFamilies.Unix,
                new[]
                {
                    OsCode.Solaris,
                    OsCode.Aix,
                    OsCode.HpUx,
                    OsCode.FreeBsd,
                    OsCode.NetBsd,
                    OsCode.OpenBsd,
                    OsCode.DragonFly,
                    OsCode.Syllable,
                    OsCode.Irix,
                    OsCode.Osf1,
                    OsCode.Inferno,
                    OsCode.ElectroBsd,
                    OsCode.Genix,
                    OsCode.Ultrix,
                    OsCode.NewsOs,
                    OsCode.NextStep,
                    OsCode.StarBladeOs,
                }.ToFrozenSet()
            },
            { OsFamilies.WebTv, new[] { OsCode.WebTv }.ToFrozenSet() },
            { OsFamilies.Windows, new[] { OsCode.Windows }.ToFrozenSet() },
            {
                OsFamilies.WindowsMobile,
                new[]
                {
                    OsCode.WindowsPhone,
                    OsCode.WindowsMobile,
                    OsCode.WindowsCe,
                    OsCode.WindowsRt,
                    OsCode.WindowsIoT,
                    OsCode.KinOs,
                }.ToFrozenSet()
            },
            { OsFamilies.OtherSmartTv, new[] { OsCode.WhaleOs }.ToFrozenSet() },
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

        DesktopOsFamilies = new[]
        {
            OsFamilies.AmigaOs,
            OsFamilies.Ibm,
            OsFamilies.GnuLinux,
            OsFamilies.Mac,
            OsFamilies.Unix,
            OsFamilies.Windows,
            OsFamilies.BeOs,
            OsFamilies.ChromeOs,
        }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

        ClientHintPlatformMappings = new Dictionary<string, FrozenSet<string>>
        {
            { OsNames.GnuLinux, new[] { "Linux" }.ToFrozenSet(StringComparer.OrdinalIgnoreCase) },
            { OsNames.Mac, new[] { "MacOS" }.ToFrozenSet(StringComparer.OrdinalIgnoreCase) },
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

        FireOsVersionMappings = new Dictionary<string, string>
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

        LineageOsVersionMappings = new Dictionary<string, string>
        {
            { "16", "23" },
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
            { "4.0.4", "9.1.0" },
        }.ToFrozenDictionary();

        WindowsMinorVersionMappings = new Dictionary<int, string>
        {
            { 1, "7" },
            { 2, "8" },
            { 3, "8.1" },
        }.ToFrozenDictionary();

        AndroidApps = new[]
        {
            "com.hisense.odinbrowser",
            "com.seraphic.openinet.pre",
            "com.appssppa.idesktoppcbrowser",
            "every.browser.inc",
        }.ToFrozenSet();

        CpuArchitectureRegexes = new Dictionary<string, Regex>
        {
            {
                CpuArchitectures.Arm,
                RegexBuilder.BuildRegex(
                    "arm[ _;)ev]|.*arm$|.*arm64|aarch64|Apple ?TV|Watch ?OS|Watch1,[12]"
                )
            },
            { CpuArchitectures.LoongArch64, RegexBuilder.BuildRegex("loongarch64") },
            { CpuArchitectures.Mips, RegexBuilder.BuildRegex("mips") },
            { CpuArchitectures.SuperH, RegexBuilder.BuildRegex("sh4") },
            { CpuArchitectures.Sparc64, RegexBuilder.BuildRegex("sparc64") },
            {
                CpuArchitectures.X64,
                RegexBuilder.BuildRegex(
                    "64-?bit|WOW64|(?:Intel)?x64|WINDOWS_64|win64|.*amd64|.*x86_?64"
                )
            },
            {
                CpuArchitectures.X86,
                RegexBuilder.BuildRegex(".*32bit|.*win32|(?:i[0-9]|x)86|i86pc")
            },
        }.ToFrozenDictionary();

        ProcessorArchitectures =
        [
            CpuArchitectures.Arm,
            CpuArchitectures.LoongArch64,
            CpuArchitectures.Mips,
            CpuArchitectures.SuperH,
            CpuArchitectures.Sparc64,
            CpuArchitectures.X64,
            CpuArchitectures.X86,
        ];
    }

    public OsParser(UaDetectorOptions? uaDetectorOptions = null)
    {
        _uaDetectorOptions = uaDetectorOptions ?? new UaDetectorOptions();
        _cache = uaDetectorOptions?.Cache;
        _botParser = new BotParser(new BotParserOptions { Cache = _cache });
    }

    private static string ApplyClientHintPlatformMapping(string platform)
    {
        foreach (var clientHint in ClientHintPlatformMappings)
        {
            if (clientHint.Value.Contains(platform))
            {
                return clientHint.Key;
            }
        }

        return platform;
    }

    private static bool TryMapNameToFamily(string name, [NotNullWhen((true))] out string? result)
    {
        if (OsRegistry.OsNameMappings.TryGetValue(name, out var code))
        {
            foreach (var osFamily in OsFamilyMappings)
            {
                if (osFamily.Value.Contains(code))
                {
                    result = osFamily.Key;
                    return true;
                }
            }
        }

        result = null;
        return false;
    }

    private static bool TryGetFireOsVersion(
        string version,
        [NotNullWhen((true))] out string? result
    )
    {
        result = null;
        var index = version.IndexOf('.');

        if (index != -1)
        {
            // TODO: Remove this once net462 support is dropped
            FireOsVersionMappings.TryGetValue(version.Substring(0, index), out result);

            // FireOsVersionMappings.TryGetValue(version[..index], out result);
        }

        if (result is null)
        {
            FireOsVersionMappings.TryGetValue(version, out result);
        }

        return result is not null;
    }

    private static bool TryGetLineageOsVersion(
        string version,
        [NotNullWhen((true))] out string? result
    )
    {
        result = null;
        var index = version.IndexOf('.');

        if (index != -1)
        {
            // TODO: Remove this once net462 support is dropped
            LineageOsVersionMappings.TryGetValue(version.Substring(0, index), out result);

            // LineageOsVersionMappings.TryGetValue(version[..index], out result);
        }

        if (result is null)
        {
            LineageOsVersionMappings.TryGetValue(version, out result);
        }

        return result is not null;
    }

    private static bool TryParseCpuArchitecture(
        string userAgent,
        ClientHints clientHints,
        [NotNullWhen(true)] out string? result
    )
    {
        result = null;

        if (clientHints.Architecture?.Length > 0)
        {
            var architecture = clientHints.Architecture.ToLower();

            if (architecture.Contains("arm"))
            {
                result = CpuArchitectures.Arm;
            }
            else if (architecture.Contains("loongarch64"))
            {
                result = CpuArchitectures.LoongArch64;
            }
            else if (architecture.Contains("mips"))
            {
                result = CpuArchitectures.Mips;
            }
            else if (architecture.Contains("sh4"))
            {
                result = CpuArchitectures.SuperH;
            }
            else if (architecture.Contains("sparc64"))
            {
                result = CpuArchitectures.Sparc64;
            }
            else if (
                architecture.Contains("x64")
                || (architecture.Contains("x86") && clientHints.Bitness == "64")
            )
            {
                result = CpuArchitectures.X64;
            }
            else if (architecture.Contains("x86"))
            {
                result = CpuArchitectures.X86;
            }

            if (result?.Length > 0)
            {
                return true;
            }
        }

        foreach (var cpuArchitecture in ProcessorArchitectures)
        {
            if (
                CpuArchitectureRegexes.TryGetValue(cpuArchitecture, out var regex)
                && regex.IsMatch(userAgent)
            )
            {
                result = cpuArchitecture;
                break;
            }
        }

        return result is not null;
    }

    private bool TryParseOsFromClientHints(
        ClientHints clientHints,
        [NotNullWhen(true)] out CommonOsInfo? result
    )
    {
        if (clientHints.Platform is null or { Length: 0 })
        {
            result = null;
            return false;
        }

        string name = ApplyClientHintPlatformMapping(clientHints.Platform);
        name = name.CollapseSpaces();

        if (OsRegistry.OsNameMappings.TryGetValue(name, out var code))
        {
            name = OsRegistry.OsCodeMappings[code];
        }
        else
        {
            result = null;
            return false;
        }

        string? version = clientHints.PlatformVersion;

        if (name == OsNames.Windows && version?.Length > 0)
        {
            var versionParts = version.Split('.');
            int majorVersion =
                versionParts.Length > 0 && int.TryParse(versionParts[0], out var major) ? major : 0;
            int minorVersion =
                versionParts.Length > 1 && int.TryParse(versionParts[1], out var minor) ? minor : 0;

            switch (majorVersion)
            {
                case 0 when minorVersion != 0:
                    WindowsMinorVersionMappings.TryGetValue(minorVersion, out version);
                    break;
                case > 0
                and <= 10:
                    version = "10";
                    break;
                case > 10:
                    version = "11";
                    break;
            }
        }

        // On Windows, version 0.0.0 can represent 7, 8, or 8.1, so it is set to null.
        if (
            name != OsNames.Windows
            && version?.Length > 0
            && version != "0.0.0"
            && ParserExtensions.TryCompareVersions(version, "0", out var comparisonResult)
            && comparisonResult == 0
        )
        {
            version = null;
        }

        result = new CommonOsInfo
        {
            Name = name,
            Version = ParserExtensions.BuildVersion(version, _uaDetectorOptions.VersionTruncation),
        };

        return true;
    }

    private bool TryParseOsFromUserAgent(
        string userAgent,
        [NotNullWhen(true)] out CommonOsInfo? result
    )
    {
        Match? match = null;
        Os? os = null;

        foreach (var osPattern in OperatingSystems)
        {
            match = osPattern.Regex.Match(userAgent);

            if (match.Success)
            {
                os = osPattern;
                break;
            }
        }

        if (os is null || match is null || !match.Success)
        {
            result = null;
            return false;
        }

        string name = ParserExtensions.FormatWithMatch(os.Name, match);

        if (OsRegistry.OsNameMappings.TryGetValue(name, out var code))
        {
            name = OsRegistry.OsCodeMappings[code];
        }
        else
        {
            result = null;
            return false;
        }

        var version = ParserExtensions.BuildVersion(
            os.Version,
            match,
            _uaDetectorOptions.VersionTruncation
        );

        if (os.Versions?.Count > 0)
        {
            foreach (var osVersion in os.Versions)
            {
                match = osVersion.Regex.Match(userAgent);

                if (match.Success)
                {
                    version = ParserExtensions.BuildVersion(
                        osVersion.Version,
                        match,
                        _uaDetectorOptions.VersionTruncation
                    );
                    break;
                }
            }
        }

        result = new CommonOsInfo { Name = name, Version = version };
        return true;
    }

    public bool TryParse(string userAgent, [NotNullWhen(true)] out OsInfo? result)
    {
        return TryParse(userAgent, ImmutableDictionary<string, string?>.Empty, out result);
    }

    public bool TryParse(
        string userAgent,
        IDictionary<string, string?> headers,
        [NotNullWhen(true)] out OsInfo? result
    )
    {
        var clientHints = ClientHints.Create(headers);

        if (!_uaDetectorOptions.DisableBotDetection && _botParser.IsBot(userAgent))
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
        [NotNullWhen(true)] out OsInfo? result
    )
    {
        string name;
        string? version;

        TryParseOsFromUserAgent(userAgent, out var osFromUserAgent);

        if (TryParseOsFromClientHints(clientHints, out var osFromClientHints))
        {
            name = osFromClientHints.Name;
            version = osFromClientHints.Version;

            if (osFromUserAgent is not null)
            {
                TryMapNameToFamily(osFromUserAgent.Name, out string? familyFromUserAgent);

                // If no version is provided in the client hints, use the version from the user agent,
                // provided the OS family matches.
                if (
                    osFromClientHints.Version is null or { Length: 0 }
                    && TryMapNameToFamily(name, out var familyFromName)
                    && familyFromName == familyFromUserAgent
                )
                {
                    version = osFromUserAgent.Version;
                }

                // On Windows, version 0.0.0 can represent either 7, 8, or 8.1
                if (name == OsNames.Windows && version == "0.0.0")
                {
                    version = osFromUserAgent.Version == "10" ? null : osFromUserAgent.Version;
                }

                // If the OS name from client hints matches the OS family from the user agent but differs in detail,
                // prefer the user agent's OS name for greater specificity.
                if (familyFromUserAgent == name && osFromUserAgent.Name != name)
                {
                    name = osFromUserAgent.Name;

                    switch (name)
                    {
                        case OsNames.LeafOs
                        or OsNames.HarmonyOs:
                            version = null;
                            break;
                        case OsNames.PicoOs:
                            version = osFromUserAgent.Version;
                            break;
                        case OsNames.FireOs
                            when osFromClientHints.Version?.Length > 0 && version?.Length > 0:
                        {
                            TryGetFireOsVersion(version, out version);
                            break;
                        }
                    }
                }

                switch (name)
                {
                    // In some cases, Chrome OS is reported as Linux in client hints.
                    // This is corrected only when the version matches.
                    case OsNames.GnuLinux
                        when osFromUserAgent.Name == OsNames.ChromeOs
                            && osFromClientHints.Version == osFromUserAgent.Version:
                        name = osFromUserAgent.Name;
                        break;
                    // In some cases, Chrome OS is incorrectly reported as Android in client hints.
                    case OsNames.Android when osFromUserAgent.Name == OsNames.ChromeOs:
                        name = osFromUserAgent.Name;
                        version = null;
                        break;
                    // Meta Horizon is reported as Linux in client hints.
                    case OsNames.GnuLinux when osFromUserAgent.Name == OsNames.MetaHorizon:
                        name = osFromUserAgent.Name;
                        break;
                }
            }
        }
        else if (osFromUserAgent is not null)
        {
            name = osFromUserAgent.Name;
            version = osFromUserAgent.Version;
        }
        else
        {
            result = null;
            return false;
        }

        TryMapNameToFamily(name, out var family);

        if (clientHints.App?.Length > 0)
        {
            if (name != OsNames.Android && AndroidApps.Contains(clientHints.App))
            {
                name = OsNames.Android;
                family = OsFamilies.Android;
                version = null;
            }
            else if (name != OsNames.LineageOs && clientHints.App == "org.lineageos.jelly")
            {
                name = OsNames.LineageOs;
                family = OsFamilies.Android;

                if (version?.Length > 0)
                {
                    TryGetLineageOsVersion(version, out version);
                }
            }
            else if (name != OsNames.FireOs && clientHints.App == "org.mozilla.tv.firefox")
            {
                name = OsNames.FireOs;
                family = OsFamilies.Android;

                if (version?.Length > 0)
                {
                    TryGetFireOsVersion(version, out version);
                }
            }
        }

        TryParseCpuArchitecture(userAgent, clientHints, out var cpuArchitecture);

        result = new OsInfo
        {
            Name = name,
            Code = OsRegistry.OsNameMappings[name],
            Version = version,
            CpuArchitecture = cpuArchitecture,
            Family = family,
        };

        return true;
    }

    private sealed class CommonOsInfo
    {
        public required string Name { get; init; }
        public required string? Version { get; init; }
    }
}
