using UaDetector.Registries;

namespace UaDetector.DocsGenerator.DataCollectors;

public class OsDataCollector : IDataCollector
{
    public string MarkerName => "OPERATING-SYSTEMS";
    public string Title => "Operating Systems";
    public string FileName => "operating-systems.md";

    public IEnumerable<string> CollectData()
    {
        return OsRegistry.OsCodeMappings
            .Values
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase);
    }
}
