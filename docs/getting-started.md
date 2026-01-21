## Requirements

- **.NET 9 SDK** or newer for compilation. Projects can target earlier .NET versions.
- **Visual Studio 2022** (version 17.12 or later)
- **JetBrains Rider** (version 2024.3 or later)

## Installation & Setup

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

## Quick Start

Each parser provides two `TryParse()` methods: one that accepts only the user agent string and another 
that accepts both the user agent string and a collection of HTTP headers. 
For more accurate detection, it is recommended to provide the HTTP headers.

!!! tip
    Avoid directly instantiating parsers. The first call to `TryParse()` causes a noticeable
    delay due to regular expression compilation. Register services with dependency injection 
    to prevent this runtime cost.

### Basic Usage

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

### Bot Detection

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

### Example Output

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

### Registry Access

Static registry classes offer bidirectional lookups for converting between enum codes and their corresponding string names.
The `BrowserRegistry`, `OsRegistry`, and `BrandRegistry` classes provide type-safe access to predefined values.

```c#
// Get browser name from enum code
string browserName = BrowserRegistry.GetBrowserName(BrowserCode.Safari);
// Returns: "Safari"

// Try to get browser code from name (case-insensitive)
if (BrowserRegistry.TryGetBrowserCode("Safari", out var browserCode))
{
    // Output: Browser Code: Safari
    Console.WriteLine($"Browser Code: {browserCode}");
}
else
{
    Console.WriteLine("Browser not found");
}
```

