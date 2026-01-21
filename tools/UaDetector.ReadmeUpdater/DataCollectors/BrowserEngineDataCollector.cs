using UaDetector.Parsers.Browsers;

namespace UaDetector.ReadmeUpdater.DataCollectors;

public class BrowserEngineDataCollector : IDataCollector
{
    public string MarkerName => "BROWSER-ENGINES";
    public string Title => "Browser Engines";
    public string FileName => "browser-engines.md";

    public IEnumerable<string> CollectData()
    {
        return EngineParser.EngineNames;
    }
}
