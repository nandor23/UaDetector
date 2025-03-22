using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UADetector.Models.Enums;
using UADetector.Regexes.Models;
using UADetector.Results;

namespace UADetector.Parsers;

public class BotParser : IBotParser
{
    private const string ResourceName = "Regexes.Resources.bots.yml";
    private static readonly IEnumerable<Bot> Bots;
    private static readonly Regex OverallRegex;


    static BotParser()
    {
        (Bots, OverallRegex) =
            ParserExtensions.LoadRegexesWithOverallRegex<Bot>(ResourceName, RegexPatternType.UserAgent);
    }

    public bool IsBot(string userAgent)
    {
        return OverallRegex.IsMatch(userAgent);
    }

    public bool TryParse(string userAgent, [NotNullWhen(true)] out BotInfo? result)
    {
        if (OverallRegex.IsMatch(userAgent))
        {
            foreach (var bot in Bots)
            {
                var match = bot.Regex.Match(userAgent);

                if (match.Success)
                {
                    result = new BotInfo
                    {
                        Name = bot.Name,
                        Category = bot.Category,
                        Url = bot.Url,
                        Producer = string.IsNullOrEmpty(bot.Producer?.Name) && string.IsNullOrEmpty(bot.Producer?.Url)
                            ? null
                            : new ProducerInfo { Name = bot.Producer?.Name, Url = bot.Producer?.Url, },
                    };

                    return true;
                }
            }
        }

        result = null;
        return false;
    }
}
