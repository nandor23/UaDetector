using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UaDetector.Abstractions.Models;
using UaDetector.Abstractions.Models.Internal;
using UaDetector.Attributes;

namespace UaDetector.Parsers;

public sealed partial class BotParser : IBotParser
{
    [RegexSource("Resources/bots.json")]
    internal static partial IReadOnlyList<Bot> Bots { get; }

    [CombinedRegex]
    private static partial Regex CombinedRegex { get; }

    private const string ParseCacheKeyPrefix = "bot";
    private const string IsBotCacheKeyPrefix = "isbot";
    private readonly IUaDetectorCache? _cache;

    public BotParser(BotParserOptions? botOptions = null)
    {
        _cache = botOptions?.Cache;
    }

    public bool TryParse(string userAgent, [NotNullWhen(true)] out BotInfo? result)
    {
        var cacheKey = $"{ParseCacheKeyPrefix}:{userAgent}";

        if (_cache is not null && _cache.TryGet(cacheKey, out result))
        {
            return result is not null;
        }

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

                    _cache?.Set(cacheKey, result);
                    return true;
                }
            }
        }

        result = null;
        _cache?.Set(cacheKey, result);
        return false;
    }

    public bool IsBot(string userAgent)
    {
        var cacheKey = $"{IsBotCacheKeyPrefix}:{userAgent}";

        if (_cache is not null && _cache.TryGet(cacheKey, out bool result))
        {
            return result;
        }

        result = CombinedRegex.IsMatch(userAgent);
        _cache?.Set(cacheKey, result);
        return result;
    }
}
