# UaDetector

A powerful user-agent parsing library inspired by [device-detector](https://github.com/matomo-org/device-detector).

[![Build](https://github.com/UaDetector/UaDetector/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/UaDetector/UaDetector/actions/workflows/build.yml)
[![License](https://img.shields.io/github/license/UaDetector/UaDetector)](https://www.gnu.org/licenses/lgpl-3.0.en.html)

UaDetector is a user-agent parser that identifies the browser, operating system, device, client, and even detects bots.
It is composed of several sub-parsers: `OsParser`, `BrowserParser`, `ClientParser`, and `BotParser`.
Each can be used independently if only certain information is needed from the user-agent string.

## Key Features

- **Thread Safety**: The parsers are stateless by design, making them fully thread-safe and dependency-injection friendly.
- **Optimized for Performance**: Uses compiled regular expressions and frozen dictionaries for faster pattern matching and lookup operations.
- **Predefined Values**: Static classes provide access to browser, operating system, and other related metadata.
  These include: `OsNames`, `OsFamilies`, `CpuArchitectures`, `BrowserNames`, `BrowserFamilies`, `BrowserEngines`, `BrandNames`.
- **Type-Safe Values**: Certain values are represented by enums, making them suitable for database storage.
  These include: `OsCode`, `BrowserCode`, `BrandCode`, `ClientType`, `DeviceType`, `BotCategory`.
- **Try-Parse Pattern**: Parsers implement the [Try-Parse Pattern](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/exceptions-and-performance#try-parse-pattern),
  returning a **bool** to indicate success and assigning the result to an **out** parameter.

## ⚙️ Configuration

To use UaDetector, register it in *Program.cs* with the `AddUaDetector` method.
To use a sub-parser, register it using its dedicated method: `AddOsParser`, `AddBrowserParser`, `AddClientParser`, or `AddBotParser`.
All sub-parsers, except `AddBotParser`, can be configured via *UaDetectorOptions* using the *Options* pattern as shown below.

```c#
using UaDetector;

builder.Services.AddUaDetector(options =>
{
    // Custom configuration options
    // e.g., options.VersionTruncation = VersionTruncation.Major;
});
```

| Option                | Type   | Description                                                                 |
|-----------------------|--------|-----------------------------------------------------------------------------|
| `VersionTruncation`   | `enum` | Controls how version numbers are shortened (e.g., `Major`, `Minor`, `None`) |
| `DisableBotDetection` | `bool` | Disables bot detection entirely, skipping bot-related checks and parsing    |

## 🚀 Quick Start

Each parser provides two `TryParse` methods: one that accepts only the user-agent string and another
that accepts both the user-agent string and a collection of HTTP headers.
For more accurate detection, it is recommended to provide the HTTP headers.



> **Tip**:
> Avoid directly instantiating parsers. The first initialization of UaDetector (or its sub-parsers) 
> takes a few seconds (around 1-3s). To prevent this one-time cost during runtime, register the service with 
> dependency injection, as shown earlier. This way, the instantiation will happen at application startup.

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

##  💾 Caching

To enable caching, install the [UaDetector.MemoryCache](https://www.nuget.org/packages/UaDetector.MemoryCache) package and configure it using the `UseMemoryCache` extension method:

```c#
using UaDetector;
using UaDetector.MemoryCache;

builder.Services.AddUaDetector(options =>
{
    options.UseMemoryCache();
});
```

> **Note**: For full documentation, visit the [GitHub repository](https://github.com/UaDetector/UaDetector).