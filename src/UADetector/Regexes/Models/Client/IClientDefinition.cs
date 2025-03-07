using System.Text.RegularExpressions;

namespace UADetector.Regexes.Models.Client;

internal interface IClientDefinition
{
    Regex Regex { get; init; }
    string Name { get; init; }
}
