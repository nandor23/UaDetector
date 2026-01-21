using UaDetector.Parsers;

namespace UaDetector.ReadmeUpdater.DataCollectors;

public class BotDataCollector : IDataCollector
{
    public string MarkerName => "BOTS";
    public string Title => "Bots";
    public string FileName => "bots.md";

    public IEnumerable<string> CollectData()
    {
        return BotParser.Bots.Select(x => x.Name);
    }
}
