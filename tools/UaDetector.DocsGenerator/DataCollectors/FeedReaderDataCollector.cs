using UaDetector.Parsers.Clients;

namespace UaDetector.DocsGenerator.DataCollectors;

public class FeedReaderDataCollector : IDataCollector
{
    public string MarkerName => "FEED-READERS";
    public string Title => "Feed Readers";
    public string FileName => "feed-readers.md";

    public IEnumerable<string> CollectData()
    {
        return FeedReaderParser.FeedReaders.Select(x => x.Name);
    }
}
