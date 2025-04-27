using UaDetector.Parsers;

var botParser = new BotParser();
const string userAgent = "Mozilla/5.0 (compatible; Discordbot/2.0; +https://discordapp.com)";

// Check if it's a bot
if (botParser.IsBot(userAgent))
{
    Console.WriteLine("Bot detected");
}
else
{
    Console.WriteLine("No bot detected");
}

// Get bot details if needed
if (botParser.TryParse(userAgent, out var result))
{
    Console.WriteLine('\n');
    Console.WriteLine(result);
}
