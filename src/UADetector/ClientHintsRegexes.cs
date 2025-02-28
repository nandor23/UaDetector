using System.Text.RegularExpressions;

namespace UADetector;

public static partial class ClientHintsRegexes
{
    private const string FullVersionListPattern = """^"([^"]+)"; ?v="([^"]+)"(?:, )?""";
    private const string FormFactorsPattern = """
                                              "([a-zA-Z]+)"
                                              """;


#if NET7_0_OR_GREATER
    [GeneratedRegex(FullVersionListPattern)]
    public static partial Regex FullVersionListRegex();
    
    [GeneratedRegex(FormFactorsPattern)]
    public static partial Regex FormFactorsRegex();
#else
    private static readonly Regex FullVersionListRegexInstance = new(FullVersionListPattern);
    private static readonly Regex FormFactorsRegexInstance = new(FormFactorsPattern);

    public static Regex FullVersionListRegex() => FullVersionListRegexInstance;
    public static Regex FormFactorsRegex() => FormFactorsRegexInstance;
#endif
}
