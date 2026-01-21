using UaDetector.Parsers.Browsers;
using UaDetector.Registries;

namespace UaDetector.ReadmeUpdater.DataCollectors;

public class BrowserDataCollector : IDataCollector
{
    public string MarkerName => "BROWSERS";
    public string Title => "Browsers";
    public string FileName => "browsers.md";

    public IEnumerable<string> CollectData()
    {
        return BrowserRegistry.BrowserNameMappings.Keys
            .Concat(BrowserHintParser.Hints.Values)
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }
}
