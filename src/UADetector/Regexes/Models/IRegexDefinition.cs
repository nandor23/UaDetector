using System.Text.RegularExpressions;

namespace UADetector.Regexes.Models;

public interface IRegexDefinition
{
    public Regex Regex { get; init; }
    public string Name { get; init; }
}
