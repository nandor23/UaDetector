using UaDetector.ReadmeUpdater.DataCollectors;
using UaDetector.ReadmeUpdater.Utilities;

namespace UaDetector.ReadmeUpdater.Generators;

public class DocsGenerator
{
    public void Generate(IEnumerable<IDataCollector> collectors)
    {
        var docsPath = PathLocator.GetDocsPath();
        Directory.CreateDirectory(docsPath);

        var generatedCount = 0;

        foreach (var collector in collectors)
        {
            var itemsList = collector.CollectData()
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList();
            
            var count = itemsList.Count;
            var content = string.Join(", ", itemsList);

            var markdown = $@"## {collector.Title}

**{count} {collector.Title.ToLower()} supported**

{content}
";

            var filePath = Path.Combine(docsPath, collector.FileName);
            File.WriteAllText(filePath, markdown);
            generatedCount++;
        }

        Console.WriteLine($"{generatedCount} docs files generated in {docsPath}");
    }
}
