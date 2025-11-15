# UaDetector

A powerful user-agent parsing library inspired by [device-detector](https://github.com/matomo-org/device-detector)

[![Build](https://github.com/nandor23/UaDetector/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/nandor23/UaDetector/actions/workflows/build.yml)
[![License](https://img.shields.io/github/license/nandor23/UaDetector?color=%2325b99c)](https://www.gnu.org/licenses/lgpl-3.0.en.html)

UaDetector is a user-agent parser that identifies the browser, operating system, device, client, and even detects bots.
It is composed of several sub-parsers: `OsParser`, `BrowserParser`, `ClientParser`, and `BotParser`.
Each can be used independently if only certain information is needed from the user-agent string.

## Packages

| Package                                                                             | Description                                               |
|-------------------------------------------------------------------------------------|-----------------------------------------------------------|
| [UaDetector](https://www.nuget.org/packages/UaDetector)                             | High-performance user agent parser                        |
| [UaDetector.Lite](https://www.nuget.org/packages/UaDetector.Lite)                   | Memory-efficient variant with slower parsing              |
| [UaDetector.Abstractions](https://www.nuget.org/packages/UaDetector.Abstractions)   | Shared models, enums, and constants                       |
| [UaDetector.MemoryCache](https://www.nuget.org/packages/UaDetector.MemoryCache)     | Memory cache built on Microsoft.Extensions.Caching.Memory |

## Features

- **Thread-safe**: Parsers are stateless, making them safe for dependency injection and multithreaded scenarios.
- **Fast**: Uses compiled regular expressions and frozen dictionaries for faster pattern matching and lookup operations.
- **Rich metadata**: Static classes provide access to common values: `OsNames`, `OsFamilies`, `CpuArchitectures`, `BrowserNames`, `BrowserFamilies`, `BrowserEngines`, `BrandNames`.
- **Enum support**: Values such as `OsCode`, `BrowserCode`, `BrandCode`, `ClientType`, `DeviceType`, and `BotCategory` are enums, making them suitable for database storage.
- **Try-Parse Pattern**: Parsers implement the [Try-Parse Pattern](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/exceptions-and-performance#try-parse-pattern), returning a **bool** to indicate success and assigning the result to an **out** parameter.

## Requirements

- **.NET 9 SDK** or newer for compilation. Projects can target earlier .NET versions.
- **Visual Studio 2022** (version 17.12 or later)
- **JetBrains Rider** (version 2024.3 or later)

## ⚙️ Configuration

To use UaDetector, register it in *Program.cs* with the `AddUaDetector` method.
To use a sub-parser, register it using its dedicated method: `AddOsParser`, `AddBrowserParser`, `AddClientParser`, or `AddBotParser`.
All sub-parsers, except `AddBotParser`, can be configured via *UaDetectorOptions* using the *Options* pattern as shown below.

```c#
using UaDetector;

builder.Services.AddUaDetector();
```

### Configuration Options

| Option                | Type   | Description                                                                                    |
|-----------------------|--------|------------------------------------------------------------------------------------------------|
| `VersionTruncation`   | `enum` | Controls how version numbers are shortened (e.g., `None`, `Major`, `Minor`, `Patch`, `Build`). |
| `DisableBotDetection` | `bool` | Disables bot detection entirely, skipping bot-related checks and parsing.                      |

## 🚀 Quick Start

Each parser provides two `TryParse` methods: one that accepts only the user-agent string and another
that accepts both the user-agent string and a collection of HTTP headers.
For more accurate detection, it is recommended to provide the HTTP headers.

> [!TIP]
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

        return BadRequest("Unrecognized user-agent");
    }
}
```

The `BotParser` class provides an additional `IsBot` method to determine whether a user-agent string represents a bot.

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

To enable caching, install the [UaDetector.MemoryCache](https://www.nuget.org/packages/UaDetector.MemoryCache) package and configure it using the `UseMemoryCache` extension method.

```c#
using UaDetector;
using UaDetector.MemoryCache;

builder.Services.AddUaDetector(options =>
{
    options.UseMemoryCache();
});
```

### Configuration Options

| Option                    | Default   | Description                                                                                                                             |
|---------------------------|-----------|-----------------------------------------------------------------------------------------------------------------------------------------|
| `MaxKeyLength`            | 256       | Maximum length allowed for a cache key. Entries with longer keys will not be cached.                                                    |
| `Expiration`              | `null`    | Entries will expire after this duration, regardless of how frequently they are accessed.                                                |
| `SlidingExpiration`       | `null`    | Entries will expire if they haven't been accessed within this time period. The expiration timer resets each time the entry is accessed. |
| `ExpirationScanFrequency` | 1 minute  | Interval between automatic scans that remove expired cache entries.                                                                     |

> **Note**: For full documentation, visit the [GitHub repository](https://github.com/nandor23/UaDetector).
