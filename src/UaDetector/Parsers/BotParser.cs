using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UaDetector.Regexes.Models;
using UaDetector.Results;
using UaDetector.Utils;

namespace UaDetector.Parsers;

public class BotParser : IBotParser
{
    private const string ResourceName = "Regexes.Resources.bots.json";
    internal static readonly IReadOnlyList<Bot> Bots;
    private static readonly Regex CombinedRegex;

    static BotParser()
    {
        (Bots, CombinedRegex) = RegexLoader.LoadRegexesWithCombined<Bot>(ResourceName);
    }

    public bool TryParse(string userAgent, [NotNullWhen(true)] out BotInfo? result)
    {
        if (CombinedRegex.IsMatch(userAgent))
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
                        Producer = bot.Producer is null
                            ? null
                            : new ProducerInfo
                            {
                                Name = bot.Producer?.Name,
                                Url = bot.Producer?.Url,
                            },
                    };

                    return true;
                }
            }
        }

        result = null;
        return false;
    }

    public bool IsBot(string userAgent)
    {
        return CombinedRegex.IsMatch(userAgent);
    }
}
