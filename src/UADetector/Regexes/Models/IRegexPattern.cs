namespace UADetector.Regexes.Models;

public interface IRegexPattern
{
    public string Name { get; init; }
    public string Regex { get; init; }
}
