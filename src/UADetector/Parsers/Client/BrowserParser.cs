using System.Diagnostics.CodeAnalysis;

using UADetector.Regexes.Models.Client;
using UADetector.Results;

namespace UADetector.Parsers.Client;

internal class BrowserParser : BaseClientParser<Browser>
{
    private const string ResourceName = "Regexes.Resources.Client.browsers.yml";


    public override bool TryParse(string userAgent, [NotNullWhen(true)] out ClientInfo? result)
    {
        throw new NotImplementedException();
    }
}
