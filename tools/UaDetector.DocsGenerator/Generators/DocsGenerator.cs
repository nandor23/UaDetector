using System.Text;
using UaDetector.DocsGenerator.DataCollectors;
using UaDetector.DocsGenerator.Utilities;

namespace UaDetector.DocsGenerator.Generators;

public static class DocsGenerator
{
    public static void Generate(IEnumerable<IDataCollector> collectors)
    {
        var docsPath = Path.GetDirectoryName(PathLocator.GetDocsPath())!;
        var filePath = Path.Combine(docsPath, "detection-capabilities.md");

        var sb = new StringBuilder();
        sb.AppendLine("# Detection Capabilities");
        sb.AppendLine();

        foreach (var collector in collectors)
        {
            var itemsList = collector
                .CollectData()
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var count = itemsList.Count;
            var content = string.Join(", ", itemsList);

            sb.AppendLine($"## {collector.Title}");
            sb.AppendLine();
            sb.AppendLine($"**{count} {collector.Title.ToLower()} supported**");
            sb.AppendLine();
            sb.AppendLine(content);
            sb.AppendLine();
        }

        File.WriteAllText(filePath, sb.ToString());
        Console.WriteLine($"detection-capabilities.md generated in {docsPath}");
    }
}
