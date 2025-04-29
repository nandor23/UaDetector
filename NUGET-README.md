# UaDetector

A powerful user-agent parsing library inspired by [device-detector](https://github.com/matomo-org/device-detector).

[![Build](https://github.com/UaDetector/UaDetector/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/UaDetector/UaDetector/actions/workflows/build.yml)
[![License](https://img.shields.io/github/license/UaDetector/UaDetector)](https://www.gnu.org/licenses/lgpl-3.0.en.html)

UaDetector is a user-agent parsing library that identifies the browser, operating system, device, client, and even detects bots.

It is composed of several sub-parsers: `OsParser`, `BrowserParser`, `ClientParser`, and `BotParser`.
Each can be used independently if only certain information is needed from the user-agent string.

## Key Features

- **Thread Safety**: The parsers are stateless by design, making them fully thread-safe and dependency-injection friendly.
- **Optimized for Performance**: Uses compiled regular expressions and frozen dictionaries for faster pattern matching and lookup operations.
- **Predefined Values**: Static classes provide access to browser, operating system, and other related metadata.
  These include: `OsNames`, `OsFamilies`, `OsPlatformTypes`, `BrowserNames`, `BrowserFamilies`, `BrowserEngines`, `BrandNames`.
- **Type-Safe Values**: Certain values are represented by enums, making them suitable for database storage.
  These include: `OsCode`, `BrowserCode`, `BrandCode`, `ClientType`, `DeviceType`, `BotCategory`.
- **Try-Parse Pattern**: Parsers make use of the  [Try-Parse Pattern](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/exceptions-and-performance#try-parse-pattern), returning a `bool` status
  and setting the `out` parameter to `null` on failure.

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
> takes a few seconds (around 1-3s). To avoid this one-time cost during runtime, register the service with 
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
        var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToArray().FirstOrDefault());

        if (_uaDetector.TryParse(userAgent, headers, out var result))
        {
            return Ok(result);
        }
        
        return BadRequest("No matching user-agent information was found");
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

## ⚡ Benchmarks

The following benchmark compares the performance of other .NET user-agent parsing libraries.

| Method         | Mean     | Error     | StdDev    | Ratio | Allocated   | Alloc Ratio |
|--------------- |---------:|----------:|----------:|------:|------------:|------------:|
| UaDetector     | 1.844 ms | 0.0229 ms | 0.0388 ms |  1.00 |     3.54 KB |        1.00 |
| DeviceDetector | 6.084 ms | 0.0346 ms | 0.0307 ms |  3.30 |  4534.51 KB |    1,279.86 |
| UaParser       | 6.783 ms | 0.0844 ms | 0.0789 ms |  3.68 | 10794.88 KB |    3,046.84 |


The following benchmark measures the performance of different parsers within the library.

| Method                 | Mean       | Error    | StdDev   | Allocated |
|----------------------- |-----------:|---------:|---------:|----------:|
| UaDetector_TryParse    | 1,725.3 us | 30.59 us | 30.05 us |    3627 B |
| BrowserParser_TryParse | 1,266.7 us | 24.91 us | 66.06 us |    1320 B |
| ClientParser_TryParse  |   170.2 us |  3.35 us |  3.59 us |    1024 B |
| BotParser_TryParse     |   342.1 us |  4.30 us |  4.02 us |     353 B |
| BotParser_IsBot        |   333.8 us |  3.63 us |  3.40 us |         - |

> **Note**: For full documentation, visit the [GitHub repository](https://github.com/UaDetector/UaDetector).