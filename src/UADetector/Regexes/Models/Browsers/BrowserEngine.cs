using System.Text.RegularExpressions;

namespace UADetector.Regexes.Models.Browsers;

internal class BrowserEngine
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
}
