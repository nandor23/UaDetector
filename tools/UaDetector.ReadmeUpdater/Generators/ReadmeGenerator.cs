using UaDetector.ReadmeUpdater.DataCollectors;
using UaDetector.ReadmeUpdater.Utilities;

namespace UaDetector.ReadmeUpdater.Generators;

public class ReadmeGenerator
{
    public void Update(IEnumerable<IDataCollector> collectors)
    {
        var readmePath = PathLocator.GetReadmePath();
        var originalReadme = File.ReadAllText(readmePath);
        var modifiedReadme = originalReadme;

        foreach (var collector in collectors)
        {
            var data = collector.CollectData();
            var section = CollapsibleSectionGenerator.Generate(collector.Title, data);
            modifiedReadme = modifiedReadme.ReplaceMarkerContent(collector.MarkerName, section);
        }

        if (originalReadme != modifiedReadme)
        {
            modifiedReadme = modifiedReadme.ReplaceMarkerContent(
                "LAST-UPDATED",
                DateTime.Today.ToString("yyyy-MM-dd")
            );
            File.WriteAllText(readmePath, modifiedReadme);
            Console.WriteLine("README.md updated");
        }
        else
        {
            Console.WriteLine("README.md unchanged");
        }
    }
}
