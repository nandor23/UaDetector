<h1 align="center">
    <img alt="Logo" src="logo/uad-logo.svg" width="150"/>
  <br>
  UaDetector
  <br>
</h1>

<h4 align="center">A powerful user agent parser inspired by <a href="https://github.com/matomo-org/device-detector" target="_blank">Device Detector</a></h4>

<p align="center">
  <a href="https://github.com/nandor23/UaDetector/actions/workflows/build.yml"><img src="https://github.com/nandor23/UaDetector/actions/workflows/build.yml/badge.svg?branch=main" alt="Build"></a>
  <img src="https://img.shields.io/github/v/release/nandor23/UaDetector" alt="GitHub Release">
  <img src="https://img.shields.io/nuget/dt/UaDetector?color=%235c6bc0" alt="NuGet Downloads">
  <a href="https://www.gnu.org/licenses/lgpl-3.0.en.html"><img src="https://img.shields.io/github/license/nandor23/UaDetector?color=%231e8e7e" alt="License"></a>
</p>

<p align="center">
  <a href="https://nandor23.github.io/UaDetector/"><strong>📚 Documentation</strong></a>
</p>

UaDetector is a fast and precise user agent parser for .NET, built on top of the largest and most up-to-date user agent 
database from the [Matomo Device Detector](https://github.com/matomo-org/device-detector) project. It identifies 
browsers, operating systems, devices, clients, and bots.

The library is optimized for speed with in-memory caching of regular expressions and frozen dictionaries for lookup 
operations. It maintains compatibility with the original regex patterns and detection rules.

In addition to the main `UaDetector` parser, individual sub-parsers are available: `OsParser`, `BrowserParser`, 
`ClientParser`, and `BotParser`. Each can be used independently when only specific information is needed from 
the user agent string.

## Packages

| Package                                                                             | Description                                               |
|-------------------------------------------------------------------------------------|-----------------------------------------------------------|
| [UaDetector](https://www.nuget.org/packages/UaDetector)                             | User agent parser optimized for speed                     |
| [UaDetector.Lite](https://www.nuget.org/packages/UaDetector.Lite)                   | Memory-optimized variant with slower parsing speed        |
| [UaDetector.Abstractions](https://www.nuget.org/packages/UaDetector.Abstractions)   | Shared models, enums, and constants                       |
| [UaDetector.MemoryCache](https://www.nuget.org/packages/UaDetector.MemoryCache)     | Memory cache built on Microsoft.Extensions.Caching.Memory |

## Features

- **Thread-safe**: Parsers are stateless, making them safe for dependency injection and multithreaded scenarios
- **Fast**: Uses compiled regular expressions and frozen dictionaries for faster pattern matching and lookup operations
- **Rich metadata**: Static classes provide access to common values: `OsNames`, `OsFamilies`, `CpuArchitectures`, `BrowserNames`, `BrowserFamilies`, `BrowserEngines`, `BrandNames`
- **Enum support**: Values such as `OsCode`, `BrowserCode`, `BrandCode`, `ClientType`, `DeviceType`, and `BotCategory` are enums, making them suitable for database storage
- **Try-Parse Pattern**: Parsers implement the [Try-Parse Pattern](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/exceptions-and-performance#try-parse-pattern), returning a **bool** to indicate success and assigning the result to an **out** parameter

## Requirements

- **.NET 9 SDK** or newer for compilation. Projects can target earlier .NET versions.
- **Visual Studio 2022** (version 17.12 or later)
- **JetBrains Rider** (version 2024.3 or later)

## ⚙️ Configuration

Add the *UaDetector* package (from NuGet) to the project.
```bash
$ dotnet add package UaDetector
```

To use UaDetector, register it in *Program.cs* with the `AddUaDetector()` method. 
To use a sub-parser, register it using its dedicated method: `AddOsParser()`, `AddBrowserParser()`, `AddClientParser()`, or `AddBotParser()`. 
All sub-parsers, except `AddBotParser()`, can be configured via *UaDetectorOptions* using the *Options* pattern as shown below.

```c#
using UaDetector;

builder.Services.AddUaDetector();
```

### Configuration Options

| Option                | Type   | Default  | Description                                                                                    |
|-----------------------|--------|----------|------------------------------------------------------------------------------------------------|
| `VersionTruncation`   | `enum` | `Minor`  | Controls how version numbers are shortened (e.g., `None`, `Major`, `Minor`, `Patch`, `Build`). |
| `DisableBotDetection` | `bool` | `false`  | Disables bot detection entirely, skipping bot-related checks and parsing.                      |

## 🚀 Quick Start

Each parser provides two `TryParse()` methods: one that accepts only the user agent string and another 
that accepts both the user agent string and a collection of HTTP headers. 
For more accurate detection, it is recommended to provide the HTTP headers.

>[!TIP]
> Avoid directly instantiating parsers. The first call to TryParse causes a noticeable delay
> due to the creation of regular expression objects. To prevent this one-time
> cost during runtime, register the service with dependency injection, as shown earlier.

```c#
[ApiController]
public class UaDetectorController : ControllerBase
{
    private readonly IUaDetector _uaDetector;

    public UaDetectorController(IUaDetector uaDetector)
    {
        _uaDetector = uaDetector;
    }

    [HttpGet]
    [Route("ua-detector")]
    public IActionResult GetUserAgentInfo()
    {
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
        var headers = Request.Headers.ToDictionary(
            h => h.Key,
            h => h.Value.ToArray().FirstOrDefault()
        );

        if (_uaDetector.TryParse(userAgent, headers, out var result))
        {
            return Ok(result);
        }

        return BadRequest("Unrecognized user agent");
    }
}
```

The `BotParser` class provides an additional `IsBot()` method to determine whether a user agent string represents a bot.

```c#
using UaDetector.Parsers;

var botParser = new BotParser();
const string userAgent = "Mozilla/5.0 (compatible; Discordbot/2.0; +https://discordapp.com)";

if (botParser.IsBot(userAgent))
{
    Console.WriteLine("Bot detected");
}
else
{
    Console.WriteLine("No bot detected");
}
```

## 📋 Example Output

**Input:**
```
Mozilla/5.0 (Linux; Android 14; SAMSUNG SM-S926B) AppleWebKit/537.36 (KHTML, like Gecko) SamsungBrowser/23.0 Chrome/115.0.0.0 Mobile Safari/537.36
```

**Output:**
```json
{
  "os": {
    "name": "Android",
    "code": 1,
    "version": "14",
    "cpuArchitecture": null,
    "family": "Android"
  },
  "browser": {
    "name": "Samsung Browser",
    "code": 512,
    "version": "23.0",
    "family": "Chrome",
    "engine": {
      "name": "Blink",
      "version": "115.0"
    }
  },
  "client": null,
  "device": {
    "type": 1,
    "model": "Galaxy S24+",
    "brand": {
      "name": "Samsung",
      "code": 1496
    }
  },
  "bot": null
}
```

## 🗂️ Registry Access

Static registry classes offer bidirectional lookups for converting between enum codes and their corresponding string names.
The `BrowserRegistry`, `OsRegistry`, and `BrandRegistry` classes provide type-safe access to predefined values.

```c#
// Get browser name from enum code
string browserName = BrowserRegistry.GetBrowserName(BrowserCode.Safari);
// Returns: "Safari"

// Try to get browser code from name (case-insensitive)
if (BrowserRegistry.TryGetBrowserCode("Safari", out var browserCode))
{
    Console.WriteLine($"Browser Code: {browserCode}"); // Output: Browser Code: Safari
}
else
{
    Console.WriteLine("Browser not found");
}
```

##  💾 Caching

To enable caching, install the [UaDetector.MemoryCache](https://www.nuget.org/packages/UaDetector.MemoryCache) package and configure it using the `UseMemoryCache()` extension method.

```c#
using UaDetector;
using UaDetector.MemoryCache;

builder.Services.AddUaDetector(options =>
{
    options.UseMemoryCache();
});
```

### Configuration Options

| Option                    | Type        | Default                    | Description                                                                                                                                                                             |
|---------------------------|-------------|----------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `MaxKeyLength`            | `int`       | `256`                      | Maximum length allowed for a cache key. Entries with longer keys will not be cached.                                                                                                    |
| `Expiration`              | `TimeSpan?` | `null`                     | Entries will expire after this duration, regardless of how frequently they are accessed.                                                                                                |
| `SlidingExpiration`       | `TimeSpan?` | `null`                     | Entries will expire if they haven't been accessed within this time period. The expiration timer resets each time the entry is accessed.                                                 |
| `ExpirationScanFrequency` | `TimeSpan`  | <code>1&nbsp;minute</code> | Interval between automatic scans that evict expired cache entries.                                                                                                                      |
| `MaxEntries`              | `long?`     | `null`                     | Maximum number of entries allowed in the cache. When the limit is reached, least recently used entries will be evicted.                                                                 |
| `EvictionPercentage`      | `double`    | `0.05`                     | Percentage of cache entries to evict when `MaxEntries` limit is reached. Eviction runs asynchronously. When the cache is full, new entries will not be cached until eviction completes. |

## ⚡ Benchmarks

Both UaDetector and UaDetector.Lite load regular expressions into memory for parsing. 
If memory usage is a concern, UaDetector.Lite uses **5.6 times less memory** (32.38 MB vs 181.88 MB) 
than UaDetector while maintaining the same functionality at the cost of parsing speed.

### Library Comparison

| Method             | Mean     | Error     | StdDev    | Ratio | Allocated   | Alloc Ratio |
|------------------- |---------:|----------:|----------:|------:|------------:|------------:|
| UaDetector         | 2.635 ms | 0.2718 ms | 0.8014 ms |  1.00 |      2.9 KB |        1.00 |
| UaDetector.Lite    | 7.136 ms | 0.1345 ms | 0.1192 ms |  2.71 |     2.83 KB |        0.98 |
| UAParser           | 6.791 ms | 0.0842 ms | 0.0746 ms |  2.58 | 11309.33 KB |    3,900.46 |
| DeviceDetector.NET | 8.588 ms | 0.7745 ms | 2.1203 ms |  3.26 |  8621.18 KB |    2,972.82 |

### Individual Parser Performance

#### UaDetector

| Method                 | Mean       | Error     | StdDev    | Allocated |
|----------------------- |-----------:|----------:|----------:|----------:|
| UaDetector_TryParse    | 2,649.4 us | 271.72 us | 801.18 us |    2966 B |
| OsParser_TryParse      |   569.5 us |   2.20 us |   2.06 us |    1286 B |
| BrowserParser_TryParse | 1,046.6 us |   4.51 us |   4.22 us |    1895 B |
| ClientParser_TryParse  |   638.8 us |   4.97 us |   4.65 us |    1048 B |
| BotParser_TryParse     |   339.8 us |   4.03 us |   3.77 us |     276 B |
| BotParser_IsBot        |   333.8 us |   1.16 us |   1.08 us |     218 B |

#### UaDetector.Lite

| Method                 | Mean       | Error     | StdDev    | Allocated |
|----------------------- |-----------:|----------:|----------:|----------:|
| UaDetector_TryParse    | 7,196.0 us | 140.41 us | 156.07 us |    2900 B |
| OsParser_TryParse      | 1,195.6 us |  11.64 us |  10.89 us |    1287 B |
| BrowserParser_TryParse | 2,117.9 us |  35.92 us |  33.60 us |    1898 B |
| ClientParser_TryParse  |   462.3 us |   2.27 us |   2.12 us |    1049 B |
| BotParser_TryParse     |   275.6 us |   3.26 us |   3.05 us |     276 B |
| BotParser_IsBot        |   265.3 us |   0.65 us |   0.54 us |     218 B |

## 🔍 Detection Capabilities

**Last updated**: <!-- LAST-UPDATED -->2026-02-11<!-- LAST-UPDATED -->

### Operating Systems

<!-- OPERATING-SYSTEMS --><details>
<summary><strong>202</strong> operating systems supported (click to expand)</summary>
<br>

AIX, Alpine Linux, Amazon Linux, AmigaOS, Android, Android TV, AOSC OS, ArcaOS, Arch Linux, Armadillo OS, AROS, ASPLinux, Azure Linux, BackTrack, Bada, Baidu Yi, BeOS, BlackBerry OS, BlackBerry Tablet OS, blackPanther OS, Bliss OS, Brew, BrightSignOS, BSD, Caixa Mágica, CentOS, CentOS Stream, China OS, Chrome OS, Chromium OS, Clear Linux OS, ClearOS Mobile, Contiki, Coolita OS, CyanogenMod, Debian, Deepin, DragonFly, DVKBuntu, ElectroBSD, elementary OS, EulerOS, Fedora, Fenix, Fire OS, Firefox OS, Foresight Linux, Freebox, FreeBSD, FRITZ!OS, Fuchsia, FydeOS, GENIX, Gentoo, GEOS, GhostBSD, gNewSense, GNU/Linux, Google TV, GridOS, Haiku OS, HarmonyOS, HasCodingOS, HELIX OS, HP-UX, Inferno, iOS, iPadOS, IRIX, Java ME, Joli OS, KaiOS, Kali, Kanotix, KIN OS, Knoppix, KolibriOS, KreaTV, Kubuntu, LeafOS, LindowsOS, Lineage OS, Linpus, Linspire, Liri OS, Loongnix, Lubuntu, Lumin OS, LuneOS, Mac, Maemo, Mageia, Mandriva, MeeGo, Meta Horizon, MildWild, MINIX, Mint, Mocor OS, MocorDroid, moonOS, MorphOS, Motorola EZX, MRE, MTK / Nucleus, NetBSD, NEWS-OS, NeXTSTEP, Nintendo, Nintendo Mobile, Nova, NuttX, OpenBSD, OpenHarmony, openSUSE, OpenVMS, OpenVZ, OpenWrt, Opera TV, Oracle Linux, Ordissimo, Orsay, OS/2, OSF1, palmOS, Pardus, PCLinuxOS, PICO OS, Plan 9, Plasma Mobile, PlayStation, PlayStation Portable, Proxmox VE, Puffin OS, PureOS, Qtopia, Raspberry Pi OS, Raspbian, RazoDroiD, Red Hat, Red Star, RedOS, Remix OS, Resurrection Remix OS, Revenge OS, REX, RISC OS, risingOS, Rocky Linux, Roku OS, Rosa, RouterOS, RTOS & Next, Sabayon, Sailfish OS, Scientific Linux, SeewoOS, SerenityOS, Sirin OS, Slackware, Smartisan OS, Solaris, Star-Blade OS, SUSE, Syllable, Symbian, Symbian OS, Symbian OS Series 40, Symbian OS Series 60, Symbian^3, TencentOS, ThreadX, Titan OS, TiVo OS, Tizen, TmaxOS, Turbolinux, tvOS, Ubuntu, ULTRIX, UOS, VectorLinux, VIDAA, ViziOS, watchOS, Wear OS, Webian, webOS, WebTV, Whale OS, Windows, Windows CE, Windows IoT, Windows Mobile, Windows Phone, Windows RT, WoPhone, Xbox, Xubuntu, YunOS, Zenwalk, ZorinOS

</details><!-- OPERATING-SYSTEMS -->

### Browsers

<!-- BROWSERS --><details>
<summary><strong>712</strong> browsers supported (click to expand)</summary>
<br>

115 Browser, 18+ Privacy Browser, 1DM Browser, 1DM+ Browser, 2345 Browser, 360 Phone Browser, 360 Secure Browser, 7654 Browser, 7Star, ABrowse, Ace, Acoo Browser, AdBlock Browser, Adult Browser, Ai Browser, Airfind Secure Browser, Aloha Browser, Aloha Browser Lite, AltiBrowser, ALVA, Amaya, Amaze Browser, Amerigo, Amiga Aweb, Amiga Voyager, Amigo, Android Browser, Anka Browser, Anonyv Browser, ANT Fresco, ANTGalio, AOL Desktop, AOL Explorer, AOL Shield, AOL Shield Pro, Aplix, APN Browser, AppBrowzer, AppTec Secure Browser, APUS Browser, Arachne, Arc Search, Arctic Fox, Armorfly Browser, Arora, ArtisBrowser, Arvin, Ask Browser, Ask.com, Asus Browser, Atlas, Atom, Atomic Web Browser, Avant Browser, Avast Secure Browser, AVG Secure Browser, Avira Secure Browser, Awesomium, AwoX, Azka Browser, B-Line, Baidu Browser, Baidu Spark, Bang, Bangla Browser, Basic Web Browser, Basilisk, Beaker Browser, Beamrise, Belva Browser, Beonex, Berry Browser, Beyond Private Browser, BF Browser, Bitchute Browser, Biyubi, BizBrowser, Black Browser, Black Lion Browser, BlackBerry Browser, BlackHawk, Blazer, Bloket, Blue Browser, Bluefy, Bonsai, Borealis Navigator, Brave, BriskBard, BroKeep Browser, Browlser, BrowsBit, Browse Safe, BrowseHere, Browser Hup Pro, Browser Mini, BrowseX, Browspeed Browser, Browzar, Bunjalloo, BXE Browser, Byffox, Cake Browser, Camino, Catalyst, Catsxp, Cave Browser, CCleaner, Centaury, CG Browser, ChanjetCloud, Charon, ChatGPT Atlas, Chedot, Cheetah Browser, Cherry Browser, Cheshire, Chim Lac, Chowbo, Chrome, Chrome Frame, Chrome Mobile, Chrome Mobile iOS, Chrome Webview, ChromePlus, Chromium, Chromium GOST, Clario Browser, Classilla, Clear TV Browser, Cliqz, Cloak Private Browser, CM Browser, CM Mini, Coast, Coc Coc, Colibri, Colom Browser, Columbus Browser, Comet, CometBird, Comfort Browser, Comodo Dragon, Conkeror, CoolBrowser, CoolNovo, Cornowser, COS Browser, Craving Explorer, Crazy Browser, Cromite, Crow Browser, Crusta, Cunaguaro, Cyberfox, CyBrowser, Dark Browser, Dark Web, Dark Web Browser, Dark Web Private, dbrowser, DDU Browser, Debuggable Browser, Decentr, Deepnet Explorer, deg-degan, Deledao, Delta Browser, Desi Browser, DeskBrowse, Dezor, Diigo Browser, Dillo, DoCoMo, Dolphin, Dolphin Zero, Dooble, Dorado, Dot Browser, Doubao, Dragon Browser, DUC Browser, DuckDuckGo Privacy Browser, East Browser, Easy Browser, Ecosia, Edge WebView, EinkBro, Element Browser, Elements Browser, Elinks, Eolie, Epic, Espial TV Browser, EudoraWeb, EUI Browser, Every Browser, Explore Browser, eZ Browser, Falkon, Fast Browser UC Lite, Fast Explorer, Faux Browser, Fennec, fGet, Fiery Browser, Fire Browser, Firebird, Firefox, Firefox Focus, Firefox Klar, Firefox Mobile, Firefox Mobile iOS, Firefox Reality, Firefox Rocket, FireSend Browser, Fireweb, Fireweb Navigator, Flash Browser, Flast, Float Browser, Flock, Floorp, Flow, Flow Browser, Fluid, Flyperlink, FOSS Browser, Freedom Browser, FreeU, Frost, Frost+, Fulldive, G Browser, Galeon, Gener8, Ghostery Privacy Browser, GinxDroid Browser, Glass Browser, GNOME Web, GO Browser, GoBrowser, Godzilla Browser, GOG Galaxy, GoKu, Good Browser, Google Earth, Google Earth Pro, GreenBrowser, Habit Browser, Halo Browser, Harman Browser, Harmony 360 Browser, HasBrowser, Hawk Quick Browser, Hawk Turbo Browser, Headless Chrome, Helio, Herond Browser, Hexa Web Browser, HeyTapBrowser, Hi Browser, hola! Browser, Holla Web Browser, HONOR Browser, HotBrowser, HotJava, HTC Browser, Huawei Browser, Huawei Browser Mobile, HUB Browser, IBrowse, iBrowser, iBrowser Mini, iCab, iCab Mobile, IceCat, IceDragon, Iceweasel, iDesktop PC Browser, IE Browser Fast, IE Mobile, Ifbrowser, Impervious Browser, InBrowser, Incognito Browser, Indian UC Mini Browser, iNet Browser, Inspect Browser, Insta Browser, Internet Browser Secure, Internet Explorer, Internet Webbrowser, Intune Managed Browser, Involta Go, Iridium, Iron, Iron Mobile, Isivioo, IVVI Browser, Japan Browser, Jasmine, JavaFX, Jelly, Jig Browser, Jig Browser Plus, JioSphere, JUZI Browser, K-meleon, K-Ninja, K.Browser, Kapiko, Kazehakase, Keepsafe Browser, KeepSolid Browser, Keyboard Browser, Kids Safe Browser, Kindle Browser, Kinza, Kitt, Kiwi, Kode Browser, Konqueror, KUN, KUTO Mini Browser, Kylo, Ladybird, Lagatos Browser, Lark Browser, Legan Browser, Lemur Browser, Lenovo Browser, Lexi Browser, LG Browser, LieBaoFast, Light, Lightning Browser, Lightning Browser Plus, Lilo, Links, Liri Browser, LogicUI TV Browser, Lolifox, Lotus, Lovense Browser, LT Browser, LuaKit, LUJO TV Browser, Lulumi, Lunascape, Lunascape Lite, Lynket Browser, Lynx, Maelstrom, Mandarin, Maple, MarsLab Web Browser, MAUI WAP Browser, MaxBrowser, Maxthon, MaxTube Browser, mCent, Me Browser, Meizu Browser, Mercury, Mi Browser, MicroB, Microsoft Edge, Midori, Midori Lite, Minimo, Mint Browser, Mises, MixerBox AI, MMBOX XBrowser, Mmx Browser, Mobicip, Mobile Safari, Mobile Silk, Mogok Browser, Monument Browser, Motorola Internet Browser, MxNitro, Mypal, Naenara Browser, Naked Browser, Naked Browser Pro, Navigateur Web, NCSA Mosaic, NetFront, NetFront Life, NetPositive, Netscape, NetSurf, Neuro Browser, NextWord Browser, NFS Browser, Ninesky, Ninetails, Nintendo Browser, Nokia Browser, Nokia OSS Browser, Nokia Ovi Browser, NOMone VR Browser, Nook Browser, Norton Private Browser, Nova Browser, Nova Video Downloader Pro, Nox Browser, NTENT Browser, Nuanti Meta, Nuviu, Obigo, Ocean Browser, OceanHero, Oculus Browser, Odd Browser, Odin, Odin Browser, Odyssey Web Browser, Off By One, Office Browser, OH Browser, OH Private Browser, OhHai Browser, OJR Browser, OmniWeb, OnBrowser Lite, ONE Browser, Onion Browser, ONIONBrowser, Open Browser, Open Browser 4U, Open Browser fast 5G, Open Browser Lite, Open TV Browser, OpenFin, Openwave Mobile Browser, Opera, Opera Air, Opera Crypto, Opera Devices, Opera GX, Opera Mini, Opera Mini iOS, Opera Mobile, Opera Neon, Opera Next, Opera Touch, Oppo Browser, Opus Browser, Orbitum, Orca, Ordissimo, Oregano, Origin In-Game Overlay, Origyn Web Browser, OrNET Browser, Otter Browser, Owl Browser, Pale Moon, Palm Blazer, Palm Pre, Palm WebPro, Palmscape, Panda Browser, Pawxy, Peach Browser, Peeps dBrowser, Perfect Browser, Perk, Phantom Browser, Phantom.me, Phoenix, Phoenix Browser, Photon, Pi Browser, PICO Browser, Pintar Browser, PirateBrowser, PlayFree Browser, Pluma, Pocket Internet Explorer, PocketBook Browser, Polaris, Polarity, PolyBrowser, Polypane, Power Browser, Presearch, Prism, Privacy Browser, Privacy Explorer Fast Safe, PrivacyWall, Private Internet Browser, PronHub Browser, Proxy Browser, ProxyFox, Proxyium, ProxyMax, Proxynet, PSI Secure Browser, Puffin Cloud Browser, Puffin Incognito Browser, Puffin Secure Browser, Puffin Web Browser, Pure Lite Browser, Pure Mini Browser, Qazweb, Qiyu, QJY TV Browser, Qmamu, QQ Browser, QQ Browser Lite, QQ Browser Mini, QtWeb, QtWebEngine, Quark, QuarkPC, Quetta, Quick Browser, Quick Search TV, QupZilla, Qutebrowser, Qwant Mobile, Rabbit Private Browser, Raise Fast Browser, Rakuten Browser, Rakuten Web Search, Raspbian Chromium, Ray, RCA Tor Explorer, Realme Browser, Rekonq, Reqwireless WebViewer, Roccat, RockMelt, Roku Browser, Safari, Safari Technology Preview, Safe Exam Browser, Sailfish Browser, SalamWeb, Samsung Browser, Samsung Browser Lite, Savannah Browser, SavySoda, SberBrowser, Secure Browser, Secure Private Browser, SecureX, Seekee, Seewo Browser, SEMC-Browser, Seraphic Sraf, Seznam Browser, SFive, Sharkee Browser, Shiira, Sidekick, SilverMob US, SimpleBrowser, Singlebox, SiteKiosk, Sizzy, Skye, Skyfire, SkyLeap, Sleipnir, SlimBoat, Slimjet, Smart Browser, Smart Lenovo Browser, Smart Search & Web Browser, Smooz, Snap Browser, Snowshoe, Sogou Explorer, Sogou Mobile Browser, Sony Small Browser, SOTI Surf, Soul Browser, Soundy Browser, SP Browser, Spark, Spectre Browser, Splash, Sputnik Browser, Stampy Browser, Stargon, START Internet Browser, Stay Browser, Stealth Browser, Steam In-Game Overlay, Streamy, Sunflower Browser, Sunrise, Super Fast Browser, SuperBird, SuperFast Browser, surf, Surf Browser, Surfy Browser, Sushi Browser, Sweet Browser, Swiftfox, Swiftweasel, SX Browser, T+Browser, T-Browser, t-online.de Browser, TalkTo, Tao Browser, tararia, TenFourFox, Tenta Browser, Tesla Browser, Thor, Tincat Browser, Tint Browser, Tizen Browser, ToGate, Tor Browser, Total Browser, TQ Browser, TrueLocation Browser, TUC Mini Browser, Tungsten, TUSK, TV Bro, TV-Browser Internet, TweakStyle, U Browser, UBrowser, UC Browser, UC Browser HD, UC Browser Mini, UC Browser Turbo, Ui Browser Mini, Ume Browser, UPhone Browser, UR Browser, Uzbl, Vast Browser, vBrowser, VC Browser Mini Pro, VD Browser, Veera, Vegas Browser, Venus Browser, Vertex Surf, Vewd Browser, Via, Viasat Browser, VibeMate, Vision Mobile Browser, Vivaldi, Vivid Browser Mini, vivo Browser, VMS Mosaic, VMware AirWatch, Vonkeror, Vuhuv, w3m, Waterfox, Wave Browser, Wavebox, Wear Internet Browser, Web Browser & Explorer, Web Explorer, WebDiscover, Webian Shell, WebPositive, Weltweitimnetz Browser, WeTab Browser, Wexond, Whale Browser, Whale TV Browser, Wolvic, World Browser, wOSBrowser, Wukong Browser, Wyzo, X Browser Lite, X-VPN, xBrowser, XBrowser Mini, xBrowser Pro Super Fast, Xiino, XnBrowse, XNX Browser, Xooloo Internet, XPlay Browser, xStand, XtremeCast, Xvast, Yaani Browser, YAGI, Yahoo! Japan Browser, Yandex Browser, Yandex Browser Corp, Yandex Browser Lite, Yo Browser, Yolo Browser, YouBrowser, YouCare, Yuzu Browser, Zetakey, Zirco Browser, Zordo Browser, ZTE Browser, Zvu

</details><!-- BROWSERS -->

### Browser Engines

<!-- BROWSER-ENGINES --><details>
<summary><strong>20</strong> browser engines supported (click to expand)</summary>
<br>

Arachne, Blink, Clecko, Dillo, Edge, EkiohFlow, Elektra, Gecko, Goanna, iCab, KHTML, LibWeb, Maple, NetFront, NetSurf, Presto, Servo, Text-based, Trident, WebKit

</details><!-- BROWSER-ENGINES -->

### Mobile Apps

<!-- MOBILE-APPS --><details>
<summary><strong>710</strong> mobile apps supported (click to expand)</summary>
<br>

'sodes, +Simple, 1Password, 2tch, 360 Security, ACT Shield, ActionExtension, Active Cleaner, Adobe Acrobat Reader, Adobe Creative Cloud, Adobe IPM, Adobe NGL, Adobe Synchronizer, Adori, Agora, Aha Radio 2, AIDA64, Airr, Airsonic, Aka Messenger, Aka Messenger Lite, AliExpress, Alipay, All You Can Books, AllHitMusicRadio, Always Safe Security 24, Amazon Fire, Amazon Music, Amazon Shopping, Ameba, AN WhatsApp, Anchor, AnchorFM, AndroidDownloadManager, Anghami, AntennaPod, AntiBrowserSpy, Anybox, AnyDesk Remote Desktop, Anything LLM, Anytime Podcast Player, AOL, APK Downloader, Apollo, App Lock, appdb, AppGallery, Apple iMessage, Apple News, Apple Podcasts, Apple Reminders, Apple TV, Arvocast, ASUS Updater, Audacy, Audials, Audible, Audio, Audio Now, Audiobooks, Autoplius.lt, Avid Link, Awasu, Ayoba, Background Intelligent Transfer Service, Baidu Box App, Baidu Box App Lite, Baidu Input, Ballz, Bank Millenium, Battle.net, BB2C, BBC News, Be Focused, Bear, Bestgram, BetBull, BeyondPod, Bible, Bible KJV, Bifrost Wallet, Big Keyboard, Binance, Bitcoin Core, Bitsboard, Bitwarden, Blackboard, Blitz, Blue Proxy, BlueStacks, BlueWallet, Bolt, BonPrix, Bookmobile, Bookshelf, Boom, Boom360, Boomplay, Bose Music, Bose SoundTouch, BOX Video Downloader, bPod, Breez, Bridge, Broadcast, Broadway Podcast Network, Browser app, Browser-Anonymizer, BrowserPlus, Bullhorn, BuzzVideo, Calculator Hide Photos, Calculator Photo Vault, CamScanner, Canopy, Capital, capsule.fm, Castamatic, Castaway, CastBox, Castify, Castro, Castro 2, CCleaner, CGN, ChatGPT, ChMate, Chrome Update, Cici, Ciisaa, Citrix Workspace, Classic FM, Clean Master, Client, Clipbox+, Clovia, CM Security, COAF SMART Citizen, Coinbase, Cooler, Copied, Cortana, Cosmicast, Coupons & Deals, Covenant Eyes, CPod, CPU-Z, CrosswalkApp, Crypto.com DeFi Wallet, CSDN, Damus, Darty Sécurité, Daum, De Standaard, De Telegraaf, DeepL, DeepSeek, DevCasts, DeviantArt, DingTalk, DIRECTV, Discord, DManager, DNA Digiturva, Dogecoin Core, DoggCatcher, Don't Waste My Time!, douban App, DoubleTwist CloudPlayer, Doughnut, Douyin, Downcast, Downie, Download Hub, Download Manager, Downloader, Dr. Watson, DStream Air, Edge Update, Edmodo, Elisa Turvapaketti, Email Home, EMAudioPlayer, Emby Theater, Epic Games Launcher, ESET Remote Administrator, ESPN, eToro, Evolve Podcast, Expedia, Expo, EZVPN, F-Secure Mobile Security, F-Secure SAFE, Facebook, Facebook Audience Network, Facebook Groups, Facebook Lite, Facebook Messenger, Facebook Messenger Lite, faidr, Fancy Security, Fathom, FeedR, FeedStation, Fiddler Classic, Files, Fit Home, Flipboard App, Flipp, FM WhatsApp, Focus Keeper, Focus Matrix, Fountain, Freespoke, FVD - Free Video Downloader, Gaana, Garmin fenix 5X, Garmin Forerunner, GBox Helper, GBWhatsApp, Genspark, GetPodcast, Git, GitHub Desktop, GlobalProtect, GMX Mail, GO Security, GoEuro, Gold, GoldenPod, GoLoud, GoNative, Goodpods, GoodReader, Google, Google Assistant, Google Drive, Google Fiber TV, Google Go, Google Lens, Google Maps, Google Nest Hub, Google Photos, Google Play, Google Play Newsstand, Google Plus, Google Podcasts, Google Search App, Google Tag Manager, Graph Messenger, GroupMe, Guacamole, Hago, Hammel, HandBrake, HardCast, Hark Audio, Heart, HeartFocus, HeartFocus Education, HermesPod, HiCast, HideX, Hik-Connect, Himalaya, HiOS Launcher, HiSearch, HisThumbnail, HistoryHound, Hornet, Hotels.com, HP Smart, HTTP request maker, Huawei Mobile Services, Huawei Quick App Center, Hulu, HyperCatcher, iCatcher, IDM Video Download Manager, iHeartRadio, IMO HD Video Calls & Chat, IMO International Calls & Chat, Instabridge, Instacast, Instagram, Instapaper, InstaPro, iPlayTV, IPTV, IPTV Pro, iSafePlay, itel Launcher, iVoox, Jam, JaneStyle, Jaumo, Jaumo Prime, JioSaavn, Jitsi Meet, JJ2GO, Joy Launcher, Jungle Disk, Just Listen, Kajabi, KakaoTalk, KeepClean, Keeper Password Manager, Keplr, Kids Listen, KidsPod, Kik, Kimi, Kinogo.ge, KKBOX, Klara, Klarna, KPN Veilig, KUTO VPN, Kwai, Kwai Pro, KYMS - Keep Your Media Safe, L.A. Times, Landis+Gyr AIM Browser, Lark, Laughable, Lazada, LBC, LG Player, Libero Mail, Line, LinkedIn, Listen, LiSTNR, Liulo, LivU, Logi Options+, Lookr, LoseIt!, Luminary, Ma Protection, Macrium Reflect, MBolsa, Megaphone, MEmpresas, Menucast, Mercantile Bank of Michigan, Messenger Home, Messenger Lite, MessengerX, Meta Business Suite, Metacast, MetaMask, MetaTrader, MiChat, MiChat Lite, Microsoft Bing, Microsoft Copilot, Microsoft Lync, Microsoft Math Solver, Microsoft Office, Microsoft Office Access, Microsoft Office Excel, Microsoft Office Mobile, Microsoft Office OneDrive for Business, Microsoft Office OneNote, Microsoft Office PowerPoint, Microsoft Office Project, Microsoft Office Publisher, Microsoft Office Visio, Microsoft Office Word, Microsoft OneDrive, Microsoft Outlook, Microsoft Power Query, Microsoft Store, Mimir, mobile.de, MobileSMS, Mojeek, MOMO, MoonFM, mowPod, Moya, MSN, MX Player, My Bentley, My Watch Party, My World, MyTuner, nate, Naver, NAVER Dictionary, NET.mede, Netflix, News Home, News Suite by Sony, NewsArticle App, Newsly, NewsPoint, Nextcloud, NoCard VPN, NoCard VPN Lite, Nox Security, NPR, NRC, NRC Audio, NTV Mobil, NuMuKi Browser, OBS Studio, Obsidian, Odnoklassniki, OfferUp, Omshy VPN, OP.GG, Opal Travel, OpenVAS, Opera News, Opera Updater, Orange Radio, Orbot, Outcast, Overcast, Overhaul FM, Paint by Number, Palco MP3, Pandora, Papers, PeaCast, Perplexity, Petal Search, Photo Search, Photo Sherlock, Pic Collage, Pinterest, Player FM, PLAYit, Plex Media Server, Plus Messenger, Pocket Casts, Podbay, Podbean, Podcast & Radio Addict, Podcast App, Podcast Guru, Podcast Player, Podcast Republic, Podcastly, Podcat, Podcatcher Deluxe, Podchaser, Podclipper, PodCruncher, Podeo, Podfriend, Podhero, Podimo, PodKast, Podkicker, Podkicker Classic, Podkicker Pro, PodLP, PodMe, PodMN, PodNL, Podopolo, Podplay, Pods, PodTrapper, podU, Podurama, Podverse, Podvine, Podyssey, PotatoVPN, PowerShell, Procast, PugPig Bolt, Q-municate, qBittorrent, QQ, QQMusic, Quick Cast, QuickCast, Quicksilver, Quora, R, Radio Italiane, Radio Next, radio.at, radio.de, radio.dk, radio.es, radio.fr, radio.it, radio.net, radio.pl, radio.pt, radio.se, RadioApp, Radioline, RadioPublic, Rave Social, Razer Synapse, RDDocuments, readly App, Reddit, Reddit is fun, Redditor, Redline, RedReader, rekordbox, Repod, Reuters News, Reverse Image Search, Rhythmbox, RNPS Action Cards, Roblox, RoboForm, Rocket Chat, RSSDemon, RSSRadio, Rutube, SachNoi, Safari Search Helper, SafeIP, Samsung Email, Samsung Magician, Samsung Podcasts, Sanista Persian Instagram, Search By Image, SearchCraft, Seekr, ServeStream, SFR Sécurité, Sgallery, Shadow, Shadowrocket, SHAREit, ShareKaro, Shopee, ShowMe, Signal, Sina Weibo, Siri, SiriusXM, SKOUT, Skyeng, Skyeng Teachers, Skype, Skype for Business, Slack, SmartNews, smzdm, Snapchat, SnapTube, SnapU2B, Snipd, Social Media Explorer, SoFi, SogouSearch App, SohuNews, Soldier, Sonnet, Sony PlayStation 5, SOOP, SoundOn, SoundWaves, SPORT1, Spotify, Spreaker, Startsiden, Sticky Password, Stitcher, StoryShots, Stream Master, Streamlabs OBS, Strimio, Super Cleaner, Superbalist, Surfshark, Sweep, Swinsian, Swisscom Internet Security, Swoot, T2S, TalkTalk SuperSafe, Taobao, TCL Live, Teams, Telegram, Telekom Mail, Telia Trygg, Telia Turvapaketti, Tencent Docs, TeraBox, The Crossword, The Epoch Times, The New York Times, The Wall Street Journal, Theyub, Threads, Thunder, tieba, TikTok, TikTok Lite, TIM, Tinder, TiviMate, TopBuzz, TopSecret Chat, TotalAV, TownNews Now, TracePal, Tracker Connect, Trade Me, TradingView, Treble.fm, TRP Retail Locator, Tumile, TuneIn Radio, TuneIn Radio Pro, TurTc, Turtlecast, Tuya Smart Life, TV Cast, TVirl, twinkle, Twitch Studio, Twitter, Twitterrific, U-Cursos, Ubook Player, UCast, Uconnect LIVE, Uforia, Unibox, UnityPlayer, UPC Internet Security, V2Free, Vault, Viber, Victor Reader Stream 3, Victor Reader Stream New Generation, Victor Reader Stream Trek, VidJuice UniTube, Virgin Radio, VIS+, Visha, Visual Studio Code, Vodacast, VPN Monster, Vuhuv, Vuze, waipu.tv, Walla News, WatchFree+, Wattpad, Wayback Machine, Waze, Weather Home, WEB.DE Mail, WebDAV, Webex Teams, WeChat, WeChat Share Extension, WeCom, WH Questions, Whatplay, WhatsApp, WhatsApp Business, WhatsApp+2, Whisper, Windows Antivirus, Windows CryptoAPI, Windows Delivery Optimization, Windows Push Notification Services, Windows Update Agent, Wireshark, Wirtschafts Woche, Wiseplay, WNYC, Word Cookies!, WPS Office, Wynk Music, X Launcher, Xiao Yu Zhou, XING, XOS Launcher, XShare, XSplit Broadcaster, Y8 Browser, Yahoo OneSearch, Yahoo! Japan, YakYak, Yandex, Yandex Music, Yapa, Yelp Mobile, Yo WhatsApp, YouTube, Youtube Music, Zalo, Zee Business, ZEIT ONLINE, Zen, ZEPETO, Ziggo Safe Online, Zite, Zoho Chat, Zune

</details><!-- MOBILE-APPS -->

### Media Players

<!-- MEDIA-PLAYERS --><details>
<summary><strong>43</strong> media players supported (click to expand)</summary>
<br>

Alexa, Amarok, Audacious, Banshee, Boxee, Clementine, Deezer, DIGA, Downcast, FlyCast, Foobar2000, Google Podcasts, HTC Streaming Player, Hubhopper, iTunes, JHelioviewer, JRiver Media Center, Juice, Just Audio, Kasts, Kodi, MediaMonkey, Miro, MixerBox, MPlayer, mpv, Music Player Daemon, MusicBee, NexPlayer, Nightingale, QuickTime, Songbird, SONOS, Sony Media Go, Stagefright, StudioDisplay, SubStream, VLC, Winamp, Windows Media Player, XBMC, Xtream Player, YouView

</details><!-- MEDIA-PLAYERS -->

### Libraries

<!-- LIBRARIES --><details>
<summary><strong>144</strong> libraries supported (click to expand)</summary>
<br>

aiohttp, Akka HTTP, Android License Verification Library, AnyEvent HTTP, Apache HTTP Client, Apidog, Aria2, Artifactory, Axios, Azure Blob Storage, Azure Data Factory, Babashka HTTP Client, BIC Tracker, Boto3, Buildah, BuildKit, Bun, C++ REST SDK, CakePHP, CarrierWave, Containerd, containers, cPanel HTTP Client, cpp-httplib, cri-o, curl, Cygwin, Cypress, Dart, Deno, docker, Down, Electron Fetch, Emacs, Embarcadero URI Client, ESP32 HTTP Client, Faraday, fasthttp, ffmpeg, FFUF, FileDownloader, Free Download Manager, GeoIP Update, git-annex, go-container registry, Go-http-client, go-network, Google HTTP Java Client, got, GRequests, gRPC-Java, GStreamer, Guzzle (PHP HTTP Client), gvfs, hackney, Harbor registry client, Helm, HTML Parser, http.rb, HTTP:Tiny, HTTPie, httplib2, httprs, HTTPX, HTTP_Request2, Hubot, ICAP Client, Insomnia REST Client, iOS Application, IPinfo, Jakarta Commons HttpClient, Jaunt, Java, Java HTTP Client, jsdom, KaiOS Downloader, Kiwi TCMS, Kiwi TCMS API, libdnf, LibHTTP, libpod, libsoup, Libsyn, Lodestone PHP Parser, LUA OpenResty NGINX, Mandrill PHP, MatomoTracker, Mechanize, Mikrotik Fetch, Msray-Plus, Node Fetch, OKDownload Library, OkHttp, OkHttp, Open Build Service, Pa11y, Perl, Perl REST::Client, PhantomJS, PHP, PHP cURL Class, Podgrab, Postman Desktop, PRDownloader, Python Requests, Python urllib, QbHttp, quic-go, r-curl, Radio Downloader, ReactorNetty, req, request, Requests, reqwest, REST Client for Ruby, RestSharp, Resty, resty-requests, ruby, Safari View Service, ScalaJ HTTP, Skopeo, Slim Framework, SlimerJS, sqlmap, Stealer, superagent, Symfony, trafilatura, Typhoeus, uclient-fetch, Ultimate Sitemap Parser, undici, Unirest for Java, urlgrabber (yum), uTorrent, vimeo.php, webchk, Wget, Windows HTTP, WinHttp WinHttpRequest, WWW-Mechanize, XML-RPC

</details><!-- LIBRARIES -->

### Feed Readers

<!-- FEED-READERS --><details>
<summary><strong>28</strong> feed readers supported (click to expand)</summary>
<br>

Akregator, Apple PubSub, BashPodder, Breaker, castero, castget, FeedDemon, Feeddler RSS Reader, gPodder, JetBrains Omea Reader, Liferea, NetNewsWire, Newsbeuter, NewsBlur, NewsBlur Mobile App, Newsboat, Playapod, PodPuppy, PritTorrent, Pulp, QuiteRSS, ReadKit, Reeder, Reeder, RSS Bandit, RSS Junkie, RSSOwl, Stringer

</details><!-- FEED-READERS -->

### Personal Information Managers

<!-- PERSONAL-INFORMATION-MANAGERS --><details>
<summary><strong>35</strong> personal information managers supported (click to expand)</summary>
<br>

Airmail, Apple Mail, Barca, Basecamp, BathyScaphe, BlueMail, DAVdroid, eM Client, Evernote, Foxmail, Franz, Gmail, JaneView, Live5ch, Lotus Notes, Mail Master, MailApp, MailBar, Mailbird, Mailspring, Microsoft Outlook, Microsoft Outlook, NAVER Mail, Notion, Outlook Express, Postbox, Raindrop.io, Rambox Pro, SeaMonkey, Spicebird, The Bat!, Thunderbird, Windows Mail, Yahoo Mail, Yahoo! Mail

</details><!-- PERSONAL-INFORMATION-MANAGERS -->

### Device Brands

<!-- DEVICE-BRANDS --><details>
<summary><strong>2115</strong> device brands supported (click to expand)</summary>
<br>

10moons, 2E, 360, 3GNET, 3GO, 3Q, 4Good, 4ife, 5IVE, 7 Mobile, 7Ke tree, 8848, A&K, A1, A95X, AAUW, Accent, Accesstyle, ACD, Ace, Aceline, Acepad, Acer, Acteck, actiMirror, Adreamer, Adronix, Advan, Advance, Advantage Air, AEEZO, AFFIX, AfriOne, AG Mobile, AGM, AI+, AIDATA, AileTV, Ainol, Airis, Airness, AIRON, Airpha, Airtel, Airties, AirTouch, AIS, Aiuto, Aiwa, Ajib, Akai, AKIRA, Alba, Alcatel, Alcor, ALDI NORD, ALDI SÜD, Alfawise, Alienware, Aligator, All Star, AllCall, AllDocube, allente, ALLINmobile, Allview, Allwinner, Alps, alpsmart, Altech UEC, Altibox, Altice, Altimo, altron, Altus, AMA, Amazon, Amazon Basics, AMCV, AMGOO, Amigoo, Amino, Amoi, ANBERNIC, ANCEL, andersson, Andowl, Angelcare, AngelTech, Anker, Anry, ANS, ANXONIT, AOC, Aocos, Aocwei, AOpen, Aoro, Aoson, AOYODKG, ApoloSign, Apple, Aquarius, Archos, Arian Space, Arival, Ark, ArmPhone, Arnova, ARRIS, Artel, Artizlee, ArtLine, Arçelik, Asano, Asanzo, Ask, Aspera, ASSE, Assistant, astro (MY), Astro (UA), Asus, AT&T, Athesi, Atlantic Electrics, Atmaca Elektronik, ATMAN, ATMPC, ATOL, Atom, Atouch, Atozee, Attila, Atvio, Audiovox, AUPO, AURIS, Autan, AUX, Avaya, Avenzo, AVH, Avvio, Awow, AWOX, AXEN, Axioo, AXXA, Axxion, AYA, AYYA, Azeyou, AZOM, Azumi Mobile, Azupik, b2m, Backcell, BAFF, BangOlufsen, Barnes & Noble, BARTEC, BAUHN, BB Mobile, BBK, BDF, BDQ, BDsharing, Beafon, Becker, Beeline, Beelink, Beetel, Beista, Beko, Bell, Bellphone, Benco, Benesse, BenQ, BenQ-Siemens, BenWee, Benzo, Beyond, Bezkam, BGH, Biegedy, Bigben, BIHEE, BilimLand, Billion, Billow, BioRugged, Bird, Bitel, Bitmore, Bittium, Bkav, Black Bear, Black Box, Black Fox, Blackpcs, Blackphone, Blackton, Blackview, Blaupunkt, Bleck, BLISS, Blloc, Blow, Blu, Bluboo, Bluebird, Bluedot, Bluegood, BlueSky, Bluewave, BluSlate, BMAX, Bmobile, BMW, BMXC, BNCF, Bobarry, bogo, Bolva, Bookeen, Boost, Botech, Boway, bq, Bqeel, BrandCode, Brandt, BRAVE, Bravis, BrightSign, Brigmton, Brondi, BROR, BS Mobile, Bubblegum, Bundy, Bush, BuzzTV, BYD, BYJU'S, BYYBUO, C Idea, C5 Mobile, CADENA, CAGI, Caixun, CALME, Camfone, Canaima, Canal Digital, Canal+, Canguro, Capitel, Captiva, Carbon Mobile, Carrefour, Casio, Casper, Cat, Cavion, CCIT, Cecotec, Ceibal, Celcus, Celkon, Cell-C, Cellacom, CellAllure, Cellution, CENTEK, Centric, CEPTER, CG Mobile, CGV, Chainway, Changhong, CHCNAV, Cherry Mobile, Chico Mobile, ChiliGreen, China Mobile, China Telecom, Chuwi, CipherLab, Citycall, CKK Mobile, Claresta, Clarmin, CLAYTON, ClearPHONE, Clementoni, Cloud Mobile, Cloudfone, Cloudpad, Clout, Clovertek, CMF, CnM, Cobalt, Coby Kyros, Cogeco, COLORROOM, Colors, Comio, CommScope, Compal, Compaq, COMPUMAX, ComTrade Tesla, Conceptum, Concord, ConCorde, Condor, Connectce, Connex, Conquest, CONSUNG, Continental Edison, Contixo, coocaa, COOD-E, Coolpad, Coopers, CORN, Cosmote, Covia, Cowon, COYOTE, CPDEVICE, CreNova, Crescent, Crestron, Cricket, Crius Mea, Crony, Crosscall, Crown, Ctroniq, Cube, CUBOT, CUD, Cuiud, Cultraview, CVTE, Cwowdefu, CX, Cyrus, D-Link, D-Tech, Daewoo, Danew, DangcapHD, Dany, Daria, DASS, Datalogic, Datamini, Datang, Datawind, Datsun, Dawlance, Dazen, DbPhone, Dbtel, Dcode, DEALDIG, DEC, Dell, Denali, Denka, Denver, Desay, DeWalt, DEXP, DEYI, DF, DGTEC, DIALN, Dialog, Dicam, Digi, Digicel, DIGICOM, Digidragon, DIGIFORS, Digihome, Digiland, Digit, Digma, DIJITSU, DIKOM, DIMO, Dinalink, Dinax, DING DING, Diofox, DIORA, DISH, Disney, Ditecma, Diva, DiverMax, Divisat, DIXON, DL, DMOAO, DNS, DoCoMo, Doffler, Dolamee, Dom.ru, Doogee, Doopro, Doov, Dopod, Doppio, Dora, DORLAND, Doro, DPA, DRAGON, Dragon Touch, Dream Multimedia, Dreamgate, DreamStar, Droidlogic, Droxio, DSDevices, DSIC, Dtac, DUDU AUTO, Dune HD, DUNNS Mobile, DuoTV, Durabook, Duubee, Dykemann, Dyon, E-Boda, E-Ceros, E-TACHI, E-tel, Eagle, EagleSoar, EAS Electric, Easypix, EBEN, EBEST, Echo Mobiles, ecom, ECON, ECOO, EcoStar, ECS, Edanix, Edenwood, EE, EFT, EGL, EGOTEK, Ehlel, Einstein, EKINOX, EKO, Eks Mobility, EKT, ELARI, ELE-GATE, Elecson, Electroneum, ELECTRONIA, Elekta, Elektroland, Element, Elenberg, Elephone, Elevate, Elista, elit, Elong Mobile, Eltex, Ematic, Emporia, ENACOM, ENDURO, Energizer, Energy Sistem, Engel, ENIE, Enot, eNOVA, Entity, Envizen, Ephone, Epic, EPIK Learning, Epik One, Eplutus, Epson, Equator, Ergo, Ericsson, Ericy, Erisson, Essential, Essentielb, eSTAR, ETOE, Eton, eTouch, Etuline, Eudora, Eurocase, EUROLUX, Eurostar, Evercoss, Everest, Everex, Everfine, Everis, Evertek, Evolio, Evolveo, Evoo, EVPAD, EvroMedia, evvoli, EWIS, EXCEED, Exertis, Exmart, ExMobile, EXO, Explay, Express LUCK, ExtraLink, Extrem, Eyemoo, EYU, Ezio, Ezze, F&U, F+, F150, F2 Mobile, Facebook, Facetel, Facime, Fairphone, Famoco, Famous, Fantec, Fanvace, FaRao Pro, Farassoo, FarEasTone, Fengxiang, Fenoti, FEONAL, Fero, FFF SmartLife, Figgers, FiGi, FiGO, FiiO, Filimo, FILIX, FinePower, FINIX, Finlux, FireFly Mobile, FISE, Fision, FITCO, Fluo, Fly, FLYCAT, FLYCOAY, FMT, FNB, FNF, Fobem, Fondi, Fonos, FONTEL, FOODO, FORME, Formovie, Formuler, Forstar, Fortis, FortuneShip, FOSSiBOT, Four Mobile, Fourel, FOX, Foxconn, FoxxD, FPT, free, Freetel, FreeYond, FRESH, Frunsi, Fuego, FUJICOM, Fujitsu, Funai, Fusion5, Future Mobile Technology, Fxtec, G-Guard, G-PLUS, G-Tab, G-TiDE, G-Touch, G-Vill, Galactic, Galatec, Galaxy Innovations, Gamma, Garmin-Asus, Gateway, Gazal, Gazer, GDL, Geanee, Geant, Gear Mobile, Gemini, General Mobile, Genesis, Genius Devices, Geo Phone, GEOFOX, Geotel, Geotex, GEOZON, Getnord, GFive, Gfone, Ghia, Ghong, Ghost, Gigabyte, Gigaset, Gini, Ginzzu, Gionee, GIRASOLE, GlobalSec, Globex, Globmall, GlocalMe, Glofiish, GLONYX, Glory Star, GLX, GN Electronics, GOCLEVER, Gocomma, GoGEN, Gol Mobile, GOLDBERG, GoldMaster, GoldStar, Goly, Gome, GoMobile, GOODTEL, Google, Goophone, Gooweel, GOtv, Gplus, Gradiente, Graetz, Grape, Great Asia, Gree, Green Lion, Green Orange, Greentel, Gresso, Gretel, GroBerwert, Grundig, Grünberg, Gtel, GTMEDIA, GTX, Guophone, GVC Pro, H133, H96, Hafury, Haier, Haipai, Haixu, Hamlet, Hammer, Handheld, HannSpree, Hanseatic, Hanson, HAOQIN, HAOVM, Hardkernel, Harper, Hartens, Hasee, Hathway, HAVIT, HDC, HeadWolf, HEC, Heimat, Helio, Hemilton, HERO, HexaByte, Hezire, Hi, Hi Nova, Hi-Level, Hiberg, HiBy, HIGH1ONE, Highscreen, HiGrace, HiHi, HiKing, HiMax, HIPER, Hipstreet, Hiremco, Hisense, Hitachi, Hitech, HKC, HKPro, HLLO, HMD, hoco, HOFER, Hoffmann, HOLLEBERG, Homatics, Hometech, HOMII, Homtom, Honeywell, HongTop, HONKUAHG, Honor, Hoozo, Hopeland, Horion, Horizon, Horizont, Hosin, Hot Pepper, HOTACK, Hotel TV Company, HOTREALS, Hotwav, How, HP, HTC, Huadoo, Huagan, Huavi, Huawei, Hugerock, Humanware, Humax, HUMElab, Hurricane, Huskee, Hyatta, Hykker, Hyrican, Hytera, Hyundai, Hyve, I KALL, i-Cherry, I-INN, i-Joy, i-mate, i-mobile, I-Plus, iBall, iBerry, ibowin, iBrit, IconBIT, Icone Gold, iData, IDC, iDino, iDroid, iFIT, iGet, iHome Life, iHunt, Ikea, IKI Mobile, iKoMo, iKon, iKonia, IKU Mobile, iLA, iLepo, iLife, iMan, Imaq, iMars, iMI, IMO Mobile, Imose, Impression, iMuz, iNavi, INCAR, Inch, Inco, Indurama, iNew, Infiniton, InfinityPro, Infinix, InFocus, InfoKit, Infomir, InFone, Inhon, Inka, Inkti, InnJoo, Inno Hit, Innos, Innostream, iNo Mobile, Inoi, iNOVA, inovo, INQ, Insignia, INSYS, Intek, Intel, Intex, Invens, Inverto, Invin, iOcean, IOTWE, iOutdoor, iPEGTOP, iPro, iQ&T, IQM, IRA, Irbis, iReplace, Iris, iRobot, iRola, iRulu, iSafe Mobile, iStar, iSWAG, IT, iTel, iTruck, IUNI, iVA, iView, iVooMi, ivvi, iWaylink, iXTech, iYou, iZotron, Jambo, JAY-Tech, Jckkcfug, Jeep, Jeka, Jesy, JFone, Jiake, Jiayu, Jide, Jin Tu, Jinga, Jio, Jivi, JKL, Jolla, Joy, JoySurf, JPay, JREN, Jumper, Juniper Systems, Just5, JUSYEA, JVC, JXD, K-Lite, K-Touch, Kaan, Kaiomy, Kalley, Kanji, KAP, Kapsys, Karbonn, Kata, KATV1, Kazam, Kazuna, KDDI, Kempler & Strauss, Kenbo, Kendo, Keneksi, KENSHI, KENWOOD, Kenxinda, KGTEL, Khadas, Kiano, kidiby, Kingbox, Kingelon, Kingstar, Kingsun, KINGZONE, Kinstone, Kiowa, Kivi, Klipad, KMC, KN Mobile, Kocaso, Kodak, Kogan, Komu, Konka, Konrow, Koobee, Koolnee, Kooper, KOPO, Korax, Koridy, Koslam, Kraft, KREZ, KRIP, KRONO, Krüger&Matz, KT-Tech, KTC, KUBO, KuGou, Kuliao, Kult, Kumai, Kurio, KVADRA, Kvant, Kydos, Kyocera, Kyowon, Kzen, KZG, L-Max, LAGENIO, LAIQ, Land Rover, Landvo, Lanin, Lanix, Lark, Laser, Laurus, Lava, LCT, Le Pan, Leader Phone, Leagoo, Leben, LeBest, Lectrus, Ledstar, LeEco, Leelbox, Leff, Legend, Leke, Lemco, LEMFO, Lemhoov, Lenco, Lenovo, Leotec, Lephone, Lesia, Lexand, Lexibook, LG, Liberton, Lifemaxx, Lime, Lingbo, Lingwin, Linnex, Linsar, Linsay, Listo, LNMBBS, Loewe, LOGAN, Logic, Logic Instrument, Logicom, Logik, Logitech, LOKMAT, LongTV, Loview, Lovme, LPX-G, LT Mobile, Lumigon, Lumitel, Lumus, Luna, LUNNEN, LUO, Luxor, Lville, LW, LYF, LYOTECH LABS, M-Horse, M-KOPA, M-Tech, M.T.T., M3 Mobile, M4tel, MAC AUDIO, Macoox, Mafe, MAG, MAGCH, Magenta, Magicsee, Magnus, Majestic, Malata, Mango, Manhattan, Mann, Manta Multimedia, Mantra, Mara, Marshal, Mascom, Massgo, Masstel, Master-G, Mastertech, Matco Tools, Matrix, Maunfeld, Maxcom, Maxfone, Maximus, Maxtron, MAXVI, Maxwell, Maxwest, MAXX, Maze, Maze Speed, MBI, MBK, MBOX, McLaut, MDC Store, meanIT, Mecer, MECHEN, Mecool, Mediacom, Medion, MEEG, MEGA VISION, Megacable, MegaFon, MEGAMAX, Meitu, Meizu, Melrose, MeMobile, Memup, MEO, MESWAO, Meta, Metz, MEU, Microlab, MicroMax, Microsoft, Microtech, Mightier, MIIA, Minix, Mint, Mintt, Mio, Mione, mipo, Miray, Mitchell & Brown, Mito, Mitsubishi, Mitsui, MIVO, MIWANG, MIXC, MiXzo, MLLED, MLS, MMI, Mobell, Mobicel, MobiIoT, Mobiistar, Mobile Kingdom, Mobiola, Mobistel, MobiWire, Mobo, Mobvoi, Mode 1, Mode Mobile, Modecom, Mofut, Moondrop, MORTAL, Mosimosi, Motiv, Motorola, Motorola Solutions, Movic, MOVISUN, Movitel, Moxee, mPhone, Mpman, MSI, MStar, MTC, MTN, multibox, Multilaser, MultiPOS, MULTYNET, MwalimuPlus, MYFON, MyGica, MygPad, Mymaga, MyMobile, MyPhone (PH), myPhone (PL), Myria, Myros, Mystery, MyTab, MyWigo, N-one, Nabi, NABO, Nanho, Naomi Phone, NASCO, National, Navcity, Navitech, Navitel, Navon, NavRoad, NEC, Necnot, Nedaphone, Neffos, NEKO, Neo, neoCore, Neolix, Neomi, Neon IQ, Neoregent, Nesons, NetBox, Netgear, Netmak, NETWIT, NeuImage, NeuTab, NEVIR, New Balance, New Bridge, Newal, Newgen, Newland, Newman, Newsday, NewsMy, Nexa, Nexar, NEXBOX, Nexian, NEXON, NEXT, Next & NextStar, Nextbit, NextBook, NextTab, NG Optics, NGM, NGpon, Nikon, NILAIT, NINETEC, NINETOLOGY, Nintendo, nJoy, NOA, Noain, Nobby, Noblex, NOBUX, noDROPOUT, NOGA, Nokia, Nomi, Nomu, Noontec, Nordfrost, Nordmende, NORMANDE, NorthTech, Nos, Nothing, Nous, Novacom, Novex, Novey, NOVIS, NoviSea, NOVO, NTT West, NuAns, Nubia, NUU Mobile, NuVision, Nuvo, Nvidia, NYX Mobile, O+, O2, Oale, Oangcc, OASYS, Obabox, Ober, Obi, OCEANIC, Odotpad, Odys, Oilsky, OINOM, ok., Okapi, Okapia, Oking, OKSI, OKWU, Olax, Olkya, Ollee, OLTO, Olympia, OMIX, Onda, OneClick, OneLern, OnePlus, Onida, Onix, Onkyo, ONN, ONVO, ONYX BOOX, Ookee, Ooredoo, OpelMobile, Openbox, Ophone, OPPO, Opsson, Optoma, Orange, Orange Pi, Orava, Orbic, Orbita, Orbsmart, Ordissimo, Orion, OSCAL, OTT, OTTO, OUJIA, Ouki, Oukitel, OUYA, Overmax, Ovvi, Owwo, OX TAB, OYSIN, Oysters, Oyyu, OzoneHD, Pacific Research Alliance, Packard Bell, PAGRAER, Paladin, Palm, Panacom, Panasonic, Panavox, Pano, Panodic, Panoramic, Pantech, PAPYRE, Parrot Mobile, Partner Mobile, PC Smart, PCBOX, PCD, PCD Argentina, PEAQ, Pelitt, Pendoo, Penta, Pentagram, Perfeo, Phicomm, Philco, Philips, Phonemax, phoneOne, Pico, PINE64, Pioneer, Pioneer Computers, PiPO, PIRANHA, Pixela, Pixelphone, PIXPRO, Pixus, Planet Computers, Platoon, Play Now, PLDT, Ployer, Plum, PlusStyle, Pluzz, PocketBook, POCO, Point Mobile, Point of View, Polar, PolarLine, Polaroid, Polestar, PolyPad, Polytron, Pomp, Poppox, POPTEL, Porsche, Portfolio, Positivo, Positivo BGH, Powerway, PPDS, PPTV, PREMIER, Premier Star, Premio, Prestigio, PRIME, Primepad, Primux, PRISM+, Pritom, Prixton, PROFiLO, Proline, Prology, ProScan, PROSONIC, Protruly, ProVision, PULID, Punos, Purism, PVBox, Q-Box, Q-Touch, Q.Bell, QFX, Qilive, QIN, Qiuwoky, QLink, QMobile, Qnet Mobile, QTECH, Qtek, Quanta Computer, Quantum, Quatro, Qubo, Quechua, Quest, Quipus, Qumo, Qupi, Qware, QWATT, R-TV, R3Di, Rakuten, Ramos, Raspberry, Ravoz, Raylandz, Razer, RAZZ, RCA Tablets, RCT, Reach, Readboy, Realix, Realme, RED, RED-X, Redbean, Redfox, RedLine, Redway, Reeder, REGAL, RelNAT, Relndoo, Remdun, Renova, RENSO, rephone, Retroid Pocket, Revo, Revomovil, Rhino, Ricoh, Rikomagic, RIM, Ringing Bells, Rinno, Ritmix, Ritzviva, Riviera, Rivo, Rizzen, ROADMAX, Roadrover, Roam Cat, ROCH, Rocket, Rokit, Roku, Rombica, Romsat, Ross&Moor, Rover, Rover Computers, Royole, RoyQueen, RT Project, RTK, RugGear, RuggeTech, Ruggex, Ruio, Runbo, RunGee, Rupa, Ryte, S-Color, S-TELL, S2Tel, Saba, Safaricom, Sagem, Sagemcom, Saiet, SAILF, Salora, Sambox, Samsung, Samtech, Samtron, Sanei, Sankey, Sansui, Santin, SANY, Sanyo, Savio, Sber, SCHAUB LORENZ, Schneider, Schok, SCHONTECH, Scoole, Scosmos, Seatel, SEBBE, Seeken, SEEWO, SEG, Sega, SEHMAX, Selecline, Selenga, Selevision, Selfix, SEMP TCL, Sencor, Sencrom, Sendo, Senkatel, SENNA, Senseit, Senwa, SERVO, Seuic, Sewoo, SFR, SGIN, Shanling, Sharp, Shift Phones, Shivaki, Shtrikh-M, Shuttle, Sico, Siemens, Sigma, Silelis, Silent Circle, Silva Schneider, Simbans, simfer, Simply, SINGER, Singtech, Siragon, Sirin Labs, Siswoo, SK Broadband, SKG, SKK Mobile, Sky, Skyline, SkyStream, Skytech, Skyworth, Smadl, Smailo, Smart, Smart Electronic, Smart Kassel, Smart Tech, Smartab, SmartBook, SMARTEC, Smartex, Smartfren, Smartisan, Smarty, Smooth Mobile, Smotreshka, SMT Telecom, SMUX, SNAMI, SobieTech, Soda, Softbank, Soho Style, Solas, SOLE, SOLO, Solone, Sonim, SONOS, Sony, Sony Ericsson, SOSH, SoulLink, Soundmax, SOWLY, Soyes, Spark NZ, Sparx, SPC, Spectralink, Spectrum, Spice, Spider, SPURT, SQOOL, SSKY, Ssmart, Star-Light, Starlight, Starmobile, Starway, Starwind, STF Mobile, STG Telecom, Stilevs, STK, Stonex, Storex, StrawBerry, StreamSystem, STRONG, Stylo, SUAAT, Subor, Sugar, SULPICE TV, Sumvision, SUNGATE, Sunmax, Sunmi, Sunny, Sunstech, SunVan, Sunvell, SUNWIND, Super General, SuperBOX, Supermax, SuperSonic, SuperTab, SuperTV, Supra, Supraim, Surfans, Surge, Suzuki, Sveon, Swipe, SWISSMOBILITY, Swisstone, Switel, SWOFY, Syco, SYH, Sylvania, Symphony, Syrox, System76, SZ TPS, T-Mobile, T96, TADAAM, TAG Tech, Taiga System, Takara, TALBERG, Talius, Tambo, Tanix, TAUBE, TB Touch, TCL, TCL SCBC, TD Systems, TD Tech, TeachTouch, Techmade, Technicolor, Technika, TechniSat, Technopc, TECHNOSAT, TechnoTrend, TechPad, Techstorm, Techwood, Teclast, Tecno Mobile, TecToy, TEENO, Teknosa, Tele2, Telefunken, Telego, Telenor, Telia, Telit, Telkom, Telly, Telma, TeloSystems, Telpo, Temigereev, TENPLUS, Teracube, Terra, Tesco, Tesla, TETC, Tetratab, teXet, ThL, Thomson, Thuraya, TIANYU, Tibuta, Tigers, Time2, Timovi, TIMvision, Tinai, Tinmo, TiPhone, Tivax, TiVo, TJC, TJD, TOKYO, Tolino, Tone, TOOGO, Tooky, Top House, Top-Tech, TopDevice, TOPDON, Topelotek, Toplux, TOPSHOWS, Topsion, Topway, Torex, TORNADO, Torque, TOSCIDO, Toshiba, Touch Plus, Touchmate, TOX, Transpeed, Trecfone, TrekStor, Trevi, TriaPlay, Tricolor, Trident, Trifone, Trimble, Trio, Tronsmart, True, True Slim, Tsinghua Tongfang, TTEC, TTfone, TTK-TV, TuCEL, TUCSON, Tunisie Telecom, Turbo, Turbo-X, TurboKids, TurboPad, Turkcell, Tuvio, TV+, TVC, TwinMOS, TWM, Twoe, TWZ Corporation, TYD, Tymes, Türk Telekom, Türksat, U-Magic, U.S. Cellular, UD, UE, UGINE, Ugoos, Uhans, Uhappy, Ulefone, Umax, UMIDIGI, Umiio, Unblock Tech, Uniden, Unihertz, Unikalne Smartphones, Unimax, Uniqcell, Uniscope, Unistrong, Unitech, UNITED, United Group, UNIWA, Unnecto, Unnion Technologies, UNNO, Unonu, UnoPhone, Unowhy, UOOGOU, Urovo, UTime, UTOK, UTStarcom, UZ Mobile, V-Gen, V-HOME, V-HOPE, v-mobile, V7, VAIO, VALE, VALEM, VALTECH, VANGUARD, Vankyo, VANWIN, Vargo, VASOUN, Vastking, VAVA, VC, VDVD, Vega, Veidoo, Vekta, Venso, Venstar, Venturer, VEON, Verico, Verizon, Vernee, Verssed, Versus, Vertex, Vertu, Verykool, Vesta, Vestel, VETAS, Vexia, VGO TEL, ViBox, Victurio, VIDA, Videocon, Videoweb, Viendo, ViewSonic, VIIPOO, VIKUSHA, VILLAON, VIMOQ, Vinabox, Vinga, Vinsoc, Vios, Viper, Vipro, Virzo, Vision Technology, Vision Touch, Visitech, Visual Land, Vitelcom, Vitumi, Vityaz, Viumee, Vivax, VIVIBright, VIVIMAGE, Vivo, VIWA, Vizio, Vizmo, VK Mobile, VKworld, VNPT Technology, VOCAL, Vodacom, Vodafone, VOGA, VOIX, VOLIA, VOLKANO, Volla, Volt, Vonino, Vontar, Vorago, Vorcom, Vorke, Vormor, Vortex, VORTEX (RO), Voto, VOX Electronics, Voxtel, Voyo, Vsmart, Vsun, VUCATIMES, Vue Micro, Vulcan, VVETIME, Völfen, W&O, WAF, Wainyok, waipu.tv, Walker, Waltham, Walton, Waltter, WANSA, WE, We. by Loewe., Web TV, Webfleet, WeChip, Wecool, Weelikeit, Weiimi, Weimei, WellcoM, WELLINGTON, Western Digital, Weston, Westpoint, Wexler, White Mobile, Whoop, Wieppo, Wigor, Wiko, WildRed, Wileyfox, Winds, Wink, Winmax, Winnovo, Winstar, Wintouch, Wiseasy, WIWA, WizarPos, Wizz, Wolder, Wolfgang, Wolki, WONDER, Wonu, Woo, Woxter, WOZIFAN, WS, X-AGE, X-BO, X-Mobile, X-TIGI, X-View, X.Vision, X88, X96, X96Q, XB, Xcell, XCOM, Xcruiser, XElectron, XGEM, XGIMI, Xgody, Xiaodu, Xiaolajiao, Xiaomi, Xion, Xolo, Xoro, XPPen, XREAL, Xshitou, Xsmart, Xtouch, Xtratech, Xwave, XY Auto, Yandex, Yarvik, YASIN, YELLYOUTH, YEPEN, Yes, Yestel, Yezz, YIKEMI, Yoka TV, Yooz, Yota, YOTOPT, Youin, Youwei, Ytone, Yu, YU Fly, Yuandao, YUHO, YUMKEM, YUNDOO, Yuno, YunSong, Yusun, Yxtel, Z-Kai, Zaith, ZALA, Zamolxe, Zatec, Zealot, Zeblaze, Zebra, Zeeker, Zeemi, Zen, Zenek, Zentality, Zfiner, ZH&K, Zhongyu Display, Zidoo, ZIFFLER, ZIFRO, Zigo, ZIK, Zimmer, Zinox, ZIOVO, Ziox, Zonda, Zonko, Zoom, ZoomSmart, Zopo, ZTE, Zuum, Zync, ZYQ, Zyrex, ZZB, öwn

</details><!-- DEVICE-BRANDS -->

### Bots

<!-- BOTS --><details>
<summary><strong>795</strong> bots supported (click to expand)</summary>
<br>

1001FirmsBot, 2GDPR, 2ip, 360 Monitoring, 360JK, Abonti, Aboundexbot, AccompanyBot, Acoon, AdAuth, Adbeat, AddSearchBot, AddThis.com, ADMantX, ADmantX Service Fetcher, Adsbot, Adscanner, AdsTxtCrawler, adstxtlab.com, Aegis, aHrefs Bot, AhrefsSiteAudit, Ai2Bot, Ai2Bot-DeepResearchEval, Ai2Bot-Dolma, aiHitBot, Alexa Crawler, Alexa Site Audit, AliyunSecBot, Allloadin Favicon Bot, AlltheWeb, AlphaXCrawl, Amazon AdBot, Amazon ELB, Amazon Kendra, Amazon Route53 Health Check, Amazonbot, Amazonbot-Video, AmazonBuyForMe, Amorank Spider, Amzn-SearchBot, Amzn-User, Analytics SEO Crawler, Andibot, Ant, Anthropic AI, Apache, ApacheBench, Applebot, Applebot-Extended, AppSignalBot, Arachni, archive.org bot, ArchiveBot, ArchiveBox, Arquivo.pt, ARSNova Filter System, Asana, Ask Jeeves, AspiegelBot, Assetnote, Automattic Analytics, Awario, Awario, Backlink-Check.de, BacklinkCrawler, BacklinksExtendedBot, BackupLand, Baidu Spider, Bard-AI, Barkrowler, Barracuda Sentinel, BazQux Reader, BBC Forge URL Monitor, BBC Page Monitor, BDCbot, BDFetch, Better Uptime Bot, Big Sur AI, BingBot, Birdcrawlerbot, BitlyBot, BitSight, Blackboard Ally, Blackbox Exporter, Blekkobot, BLEXBot Crawler, Bloglines, Bloglovin, Blogtrottr, Bluesky, BoardReader, BoardReader Blog Indexer, Botify, Bountii Bot, BrandVerity, Bravebot, BrightBot, BrightEdge, Browsershots, BUbiNG, Buck, BuiltWith, Butterfly Robot, Bytespider, CareerBot, Castopod, Castro 2, Catchpoint, CATExplorador, ccBot crawler, CensysInspect, Character-AI, Charlotte, Chartable, ChatGLM-Spider, ChatGPT-Browser, ChatGPT-User, Chatwork LinkPreview, CheckHost, CheckMark Network, Choosito, Chrome Privacy Preserving Prefetch Proxy, Cincraw, CISPA Web Analyzer, CLASSLA-web, Claude-SearchBot, Claude-User, Claude-Web, ClaudeBot, Clickagy, Cliqzbot, CloudFlare Always Online, CloudFlare AMP Fetcher, Cloudflare Custom Hostname Verification, Cloudflare Diagnostics, Cloudflare Health Checks, Cloudflare Observatory, Cloudflare Security Insights, Cloudflare Smart Transit, Cloudflare SSL Detector, Cloudflare Traffic Manager, CloudServerMarketSpider, CMS Experiment, Cocolyzebot, Cohere AI, Cohere-Command, cohere-training-data-crawler, Collectd, colly, CommaFeed, COMODO DCV, Comscore, ContentKing, Convertify, Cookiebot, CopyvioDetector, Cortex Xpanse, Cotoyogi, Crawldad, Crawlson, Crawlspace, Crawly Project, CriteoBot, CrowdTangle, CrystalSemanticsBot, CSS Certificate Spider, CSSCheck, CybaaAgent, CybaaBot, CyberFind Crawler, Cyberscan, Cypex, Cốc Cốc Bot, DaspeedBot, Datadog Agent, DataForSeoBot, datagnionbot, Datanyze, Dataprovider, DataXu, Daum, Dazoobot, Deep SEARCH 9, Deepfield Genome, deepnoc, DeepseekBot, DepSpid, Detectify, Devin, Diffbot, Discobot, Discord Bot, Disqus, Domain Codex, Domain Re-Animator Bot, Domain Research Project, DomainAppender, DomainCrawler, Domains Project, DomainStatsBot, DomCop Bot, DotBot, Dotcom Monitor, Doximity-Diffbot, Dubbotbot, DuckAssistBot, DuckDuckBot, ducks.party, DuplexWeb-Google, DVbot, DynatraceSynthetic, Easou Spider, eCairn-Grabber, EFF Do Not Track Verifier, Elastic Synthetics, EMail Exractor, EmailWolf, Embedly, Entfer, evc-batch, Everyfeed, ExaBot, ExactSeek Crawler, Example3, Exchange check, EyeMonit, Eyeotabot, eZ Publish Link Validator, Ezgif, Ezooms, Facebook Crawler, FacebookBot, Faveeo, FediList, Feed Wrangler, Feedbin, FeedBurner, Feedly, Feedspot, Femtosearch, Fever, FHMS ITS Research Scanner, FindFiles.net, Findxbot, FirecrawlAgent, Flipboard, FontRadar, ForwardQR, fragFINN, FreeWebMonitoring, FreshRSS, Friendica, Functionize, Gaisbot, GDNP, GeedoBot, GeedoProductSearch, Gemini-AI, Gemini-Deep-Research, Genieo Web filter, Ghost Inspector, Gigablast, Gigabot, Github Camo, GitHubCopilotChat, Gluten Free Crawler, Gmail Image Proxy, Gobuster, Golfe, Goo, Google Apps Script, Google Area 120 Privacy Policy Fetcher, Google Cloud Scheduler, Google Docs, Google Favicon, Google PageSpeed Insights, Google Partner Monitoring, Google Search Console, Google Sheets, Google Slides, Google Stackdriver Monitoring, Google StoreBot, Google Structured Data Testing Tool, Google Transparency Report, Google-CloudVertexBot, Google-Document-Conversion, Google-Extended, Google-Firebase, Google-ilp, Google-NotebookLM, Google-Pinpoint, Google-Safety, GoogleAgent-Mariner, GoogleAgent-Search, Googlebot, Googlebot, Googlebot News, Gowikibot, Gozle, GPTBot, Grafana, Grammarly, Grapeshot, Gregarius, Groq-Bot, GTmetrix, GumGum Verity, hackermention, HaloBot, Hatena Bookmark, Hatena Favicon, Headline, HeartRails Capture, Heexybot, Heritrix, Heureka Feed, htmlyse, HTTPMon, httpx, HuaweiWebCatBot, HubPages, HubSpot, HubSpotContentSearchBot, HuggingFace-Bot, HypeStat, IAS Crawler, IAS Wombles, iAskBot, IbouBot, ICC-Crawler, IDG, Iframely, IIS Site Analysis, ImageSift, img2dataset, Inetdex Bot, Infegy, InfoTigerBot, Inktomi Slurp, inoreader, Inspici, InsytfulBot, Intelligence X, Interactsh, InternetMeasurement, IONOS Crawler, IP-Guide Crawler, IPIP, IPS Agent, IsItWP, iTMS, JobboerseBot, JungleKeyThumbnail, K6, KadoBot, Kagibot, Kaspersky, KeybaseBot, KeyCDN Tools, KeyCDN Tools, Keys.so, Kiwi TCMS GitOps, KlarnaBot, KomodiaBot, Konturbot, Kouio, Kozmonavt, KStandBot, l9explore, l9tcpid, LAC IA Harvester, Larbin web crawler, LastMod Bot, LCC, leak.info, LeakIX, Let's Encrypt Validation, LetSearch, Lighthouse, LightspeedSystemsCrawler, LinerBot, Linespider, LinkBloom, Linkdex Bot, LinkedIn Bot, LinkpadBot, LinkPreview, LinkWalker, LiveJournal, LTX71, Lumar, LumeWebScan, LumtelBot, Lycos, MaCoCu, MADBbot, Magpie-Crawler, MagpieRSS, Mail.Ru Bot, MakeMerryBot, Manus-User, Marginalia, MariaDB/MySQL Knowledge Base, masscan, masscan-ng, Mastodon Bot, Matomo, Meanpath Bot, Mediatoolkit Bot, Mediumbot, MegaIndex, MeltwaterNews, Meta-ExternalAds, Meta-ExternalAgent, Meta-ExternalFetcher, Meta-WebIndexer, MetaInspector, MetaJobBot, MicroAdBot, Microsoft Power Automate, Microsoft Preview, Miniature.io, MistralAI-User, Mixnode, MixRank Bot, MJ12 Bot, Mnogosearch, ModatScanner, MojeekBot, Monitor Backlinks, Monitor.Us, Monsidobot, Montastic Monitor, MoodleBot Linkchecker, Morningscore Bot, MTRobot, MuckRack, Munin, MuscatFerret, Nagios check_http, najdu.s.holubem.eu, NalezenCzBot, NameProtectBot, nbertaupete95, Neevabot, Netcraft Survey Bot, netEstate, NetLyzer FastProbe, Netpeak Checker, NetResearchServer, NetSystemsResearch, NetTrack, Netvibes, NETZZAPPEN, NewsBlur, NewsGator, Newslitbot, NiceCrawler, Nimbostratus Bot, NLCrawler, Nmap, NodePing, Notify Ninja, NovaAct, Nutch-based Bot, Nuzzel, OAI-SearchBot, oBot, Observer, Octopus, Odin, Odnoklassniki Bot, Oh Dear, Omgilibot, OmtrBot, Onalytica, OnlineOrNot Bot, OpenGraph.io, Openindex Spider, OpenLinkProfiler, OpenRobotsTXT, OpenWebSpider, Orange Bot, OSZKbot, Outbrain, Overcast Podcast Sync, OWLer, Page Modified Pinger, Pageburst, PagePeeker, PageThing, Pandalytics, PanguBot, Panscient, PaperLiBot, Paqlebot, parse.ly, PATHspider, PayPal IPN, PDR Labs, Peer39, Perplexity-User, PerplexityBot, Petal Bot, Phantomas, PHP Server Monitor, phpMyAdmin, Picsearch bot, Pigafetta, PingAdmin.Ru, Pingdom Bot, Pinterest, PiplBot, Plesk Screenshot Service, Plukkie, Pocket, Podroll Analyzer, PodUptime, Poggio-Citations, Pompos, Prerender, PritTorrent, Probely, Project Patchwatch, Project Resonance, Prometheus, ProRata, PRTG Network Monitor, Punk Map, QualifiedBot, Quantcast, QuerySeekerSpider, Quora Bot, Quora Link Preview, Qwantbot, Rainmeter, RamblerMail Image Proxy, RavenCrawler, RecordedFuture, Reddit Bot, RedekenBot, RenovateBot, Replicate-Bot, Repo Lookout, ReqBin, Research JLU, Research Scan, researchcyber.net, Riddler, RocketMonitorBot, Rogerbot, ROI Hunter, RSiteAuditor, RSSRadio Bot, RunPod-Bot, RuxitSynthetic, Ryowl, SabsimBot, SafeDNSBot, Sandoba//Crawler, SBIder, SBIntuitionsBot, Scamadviser External Hit, Scooter, Scraping Robot, Scrapy, Screaming Frog SEO Spider, ScreenerBot, Sectigo DCV, security.txt scanserver, SecurityHeaders, Seekport, Sellers.Guide, semaltbot, Semantic Scholar Bot, Semrush Reputation Management, SemrushBot, Sensika Bot, Sentry Bot, Senuto, Seobility, SEOENGBot, SEOkicks, SeolytBot, Seoscanners.net, SERankingBacklinksBot, Serendeputy Bot, Serenety, serpstatbot, Server Density, Seznam Bot, Seznam Email Proxy, Seznam Zbozi.cz, sfFeedReader, ShapBot, ShopAlike, Shopify Partner, ShopWiki, Sider, SilverReader, Simbiat Software, SimplePie, Sirdata, SISTRIX Crawler, SISTRIX Optimizer, Site24x7 Defacement Monitor, Site24x7 Website Monitoring, SiteAuditBot, Sitebulb, SiteCheckerBotCrawler, Siteimprove, SitemapParser-VIPnytt, SiteOne Crawler, SiteScore, SiteSucker, Sixy.ch, Skype URI Preview, Slackbot, SMTBot, Snap URL Preview Service, Snapchat Ads, Snapchat Proxy, SnoopSecInspect, Sogou Spider, Soso Spider, Sparkler, Spawning AI, Speedy, SpiderLing, Spinn3r, SplitSignalBot, Spotify, Sprinklr, Sputnik Bot, Sputnik Favicon Bot, Sputnik Image Bot, sqalix, SSL Labs, start.me, Startpagina Linkchecker, Statista, StatOnline.ru, StatusCake, StatusNestBacklinkSpider, Steam Chat URL Lookup, Steve Bot, Stract, Sublinq, Substack Content Fetch, SuggestBot, Superfeedr Bot, SurdotlyBot, Survey Bot, svEye, Swiftbot, Swisscows Favicons, Synapse, t3versions, Taboolabot, TactiScout, Tag Inspector, Tarmot Gezgin, tchelebi, TelegramBot, Tenable.asm, TerraCotta, TestCrawler, The British Library Legal Deposit Bot, The Trade Desk Content, theoldreader, ThousandEyes, TigerBot, TikTokSpider, Timpibot, TinEye, TinEye Crawler, Tiny Tiny RSS, TLSProbe, Together-Bot, TraceMyFile, Trendiction Bot, Trendsmap, TurnitinBot, TweetedTimes Bot, Tweetmeme Bot, Twingly Recon, Twitterbot, Twurly, UCSB Network Measurement, UGAResearchAgent, UkrNet Mail Proxy, uMBot, UniversalFeedParser, Uptime-Kuma, Uptimebot, UptimeRobot, Uptimia, URLAppendBot, URLinspector, URLSuMaBot, Vagabondo, ValidBot, Valimail, Velen Public Web Crawler, Vercel Bot, VertexWP, VeryHip, Viber Url Downloader, VirusTotal Cloud, Visual Site Mapper Crawler, VK Robot, VK Share Button, VORTEX, VU Server Health Scanner, vuhuvBot, W3C CSS Validator, W3C I18N Checker, W3C Link Checker, W3C Markup Validation Service, W3C MobileOK Checker, W3C P3P Validator, W3C Unified Validator, WanscannerBot, Wappalyzer, WARDBot, WDG HTML Validator, WebbCrawler, WebCEO, WebDataStats, Webliobot, WebMon, WebMoney Advisor, Weborama, WebPageTest, WebPros, Website-info, WebSitePulse, WebThumbnail, webtru, Webwiki, webzio, webzio-extended, WellKnownBot, WeSEE:Search, WeViKaBot, WhatCMS, WhatsMyIP.org, WhereGoes, Who.is Bot, Wibybot, WikiDo, Willow Internet Crawler, WireReaderBot, WooRank, WooRank, WordPress, WordPress.com mShots, Workona, Wotbox, wp.com feedbot, WPMU DEV, xAI-Bot, XenForo, XoviBot, xtate, YaCy, Yahoo Gemini, Yahoo! Cache System, Yahoo! Japan, Yahoo! Link Preview, Yahoo! Mail Proxy, Yahoo! Slurp, YaK, YandexAdditionalBot, YandexBot-MirrorDetector, Yeti/Naverbot, YioopBot, yoozBot, Yottaa Site Monitor, YouBot, Youdao Bot, Yourls, Yunyun Bot, Zaldamo, ZanistaBot, Zao, Ze List, Zeno, zgrab, Zookabot, ZoomBot, ZoominfoBot, Zotero Translation Server, ZumBot

</details><!-- BOTS -->
