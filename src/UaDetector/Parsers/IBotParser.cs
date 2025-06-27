using System.Diagnostics.CodeAnalysis;
using UaDetector.Models;

namespace UaDetector.Parsers;

public interface IBotParser
{
    bool TryParse(string userAgent, [NotNullWhen(true)] out BotInfo? result);
    bool IsBot(string userAgent);
}
