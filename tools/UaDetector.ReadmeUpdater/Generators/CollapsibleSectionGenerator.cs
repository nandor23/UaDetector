namespace UaDetector.ReadmeUpdater.Generators;

public static class CollapsibleSectionGenerator
{
    public static string Generate(string title, IEnumerable<string> items)
    {
        var itemsList = items.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList();
        var count = itemsList.Count;
        var content = string.Join(", ", itemsList);

        return $"""
            <details>
            <summary><strong>{count}</strong> {title.ToLower()} supported (click to expand)</summary>
            <br>

            {content}

            </details>
            """;
    }
}
