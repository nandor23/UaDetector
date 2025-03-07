using System.Text.RegularExpressions;

namespace UADetector.Regexes.Models.Client;

internal interface IClient
{
    Regex Regex { get; init; }
    string Name { get; init; }
}
