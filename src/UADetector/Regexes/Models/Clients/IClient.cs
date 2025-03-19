using System.Text.RegularExpressions;

namespace UADetector.Regexes.Models.Clients;

internal interface IClient
{
    Regex Regex { get; init; }
    string Name { get; init; }
}
