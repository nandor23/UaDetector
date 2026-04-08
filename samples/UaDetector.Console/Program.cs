using UaDetector.Abstractions.Enums;
using UaDetector.Parsers;
using UaDetector.Registries;

void SampleBotDetection()
{
    var botParser = new BotParser();
    const string userAgent = "Mozilla/5.0 (compatible; Discordbot/2.0; +https://discordapp.com)";

    Console.WriteLine("=== Bot Detection Sample ===");

    if (botParser.IsBot(userAgent))
    {
        Console.WriteLine("Bot detected");
    }
    else
    {
        Console.WriteLine("No bot detected");
    }

    if (botParser.TryParse(userAgent, out var result))
    {
        Console.WriteLine("Details: " + result);
    }

    Console.WriteLine();
}

void SampleBrowserRegistry()
{
    Console.WriteLine("=== Browser Registry Sample ===");

    if (BrowserRegistry.TryGetBrowserName(BrowserCode.Safari, out var browserName))
    {
        Console.WriteLine($"Browser Name: {browserName}");
    }
    else
    {
        Console.WriteLine("Browser name not found");
    }

    if (BrowserRegistry.TryGetBrowserCode("Safari", out var browserCode))
    {
        Console.WriteLine($"Browser Code: {browserCode}");
    }
    else
    {
        Console.WriteLine("Browser code not found");
    }

    Console.WriteLine();
}

SampleBotDetection();
SampleBrowserRegistry();
