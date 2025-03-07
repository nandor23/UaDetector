using System.Text.RegularExpressions;

namespace UADetector.Regexes.Models.Client;

internal sealed class Browser : IClientDefinition
{
    public required Regex Regex { get; init; }
    public required string Name { get; init; }
}
