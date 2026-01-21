using UaDetector.Parsers.Clients;

namespace UaDetector.DocsGenerator.DataCollectors;

public class MediaPlayerDataCollector : IDataCollector
{
    public string MarkerName => "MEDIA-PLAYERS";
    public string Title => "Media Players";
    public string FileName => "media-players.md";

    public IEnumerable<string> CollectData()
    {
        return MediaPlayerParser.MediaPlayers.Select(x => x.Name);
    }
}
