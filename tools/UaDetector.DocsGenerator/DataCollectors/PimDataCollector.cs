using UaDetector.Parsers.Clients;

namespace UaDetector.DocsGenerator.DataCollectors;

public class PimDataCollector : IDataCollector
{
    public string MarkerName => "PERSONAL-INFORMATION-MANAGERS";
    public string Title => "Personal Information Managers";
    public string FileName => "personal-information-managers.md";

    public IEnumerable<string> CollectData()
    {
        return PimParser.PersonalInformationManagers.Select(x => x.Name);
    }
}
