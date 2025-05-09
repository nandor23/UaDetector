using System.Diagnostics.CodeAnalysis;
using UaDetector.Results;

namespace UaDetector.Parsers;

public interface IBotParser
{
    bool TryParse(string userAgent, [NotNullWhen(true)] out BotInfo? result);
    bool IsBot(string userAgent);
}
