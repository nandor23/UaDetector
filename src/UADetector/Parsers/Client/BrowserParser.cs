using System.Diagnostics.CodeAnalysis;

using UADetector.Regexes.Models.Client;
using UADetector.Results;
using UADetector.Utils;

namespace UADetector.Parsers.Client;

internal class BrowserParser : BaseClientParser<Browser>
{
    private const string ResourceName = "Regexes.Resources.Client.browsers.yml";

    private static readonly IEnumerable<Browser> BrowserRegexes =
        ParserExtensions.LoadRegexes<Browser>(ResourceName);

    private bool TryParseBrowserFromClientHints(
        ClientHints clientHints,
        [NotNullWhen(true)] out ClientInfo? result
    )
    {
        if (clientHints.FullVersionList.Count == 0)
        {
            result = null;
            return false;
        }

        foreach (var fullVersion in clientHints.FullVersionList)
        {
            if (ParserExtensions.TryMapPlatformToOsName(fullVersion.Key, out var osName))
            {
                osName.CollapseSpaces();
                
            }
        }

        throw new NotImplementedException();
    }


    public override bool TryParse(
        string userAgent,
        ClientHints? clientHints,
        [NotNullWhen(true)] out ClientInfo? result
    )
    {
        throw new NotImplementedException();
    }
}
