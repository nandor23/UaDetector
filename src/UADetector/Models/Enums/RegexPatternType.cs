namespace UADetector.Models.Enums;

public enum RegexPatternType
{
    /// <summary>
    /// No additional rules are applied. The regex is used as-is.
    /// </summary>
    None,

    /// <summary>
    /// Represents the rules for matching user agent strings
    /// </summary>
    UserAgent,
}
