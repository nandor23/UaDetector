using System.Text.RegularExpressions;

namespace UADetector.Regexes.Models;

internal interface IRegexDefinition
{
    public Regex Regex { get; init; }
}
