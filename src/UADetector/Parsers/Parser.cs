namespace UADetector.Parsers;

public abstract class Parser
{
    /// <summary>
    /// Holds the path to the yml file containing regexes
    /// </summary>
    protected string FixtureFile { get; set; }

    /// <summary>
    /// Holds the internal name of the parser
    /// Used for caching
    /// </summary>
    protected string ParserName { get; set; }

    /// <summary>
    /// Holds the user agent to be parsed
    /// </summary>
    protected string UserAgent { get; set; }

    /// <summary>
    /// Holds the client hints to be parsed
    /// </summary>
    // protected ClientHints ClientHints = null;

    /// <summary>
    /// Contains a list of mappings from names we use to known client hint values
    /// </summary>
    // protected static Dictionary<string, string[]> ClientHintMapping = [];

}
