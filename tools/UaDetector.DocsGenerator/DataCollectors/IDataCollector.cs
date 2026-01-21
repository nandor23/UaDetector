namespace UaDetector.DocsGenerator.DataCollectors;

public interface IDataCollector
{
    string MarkerName { get; }
    string Title { get; }
    string FileName { get; }
    IEnumerable<string> CollectData();
}
