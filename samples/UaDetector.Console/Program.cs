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

if (botParser.TryParse(userAgent, out var result))
{
    Console.WriteLine();
    Console.WriteLine(result);
}
