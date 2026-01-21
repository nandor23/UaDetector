using UaDetector.Parsers.Clients;

namespace UaDetector.ReadmeUpdater.DataCollectors;

public class LibraryDataCollector : IDataCollector
{
    public string MarkerName => "LIBRARIES";
    public string Title => "Libraries";
    public string FileName => "libraries.md";

    public IEnumerable<string> CollectData()
    {
        return LibraryParser.Libraries.Select(x => x.Name);
    }
}
