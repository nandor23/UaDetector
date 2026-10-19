namespace UaDetector.DocsGenerator.Generators;

public static class SupportedListGenerator
{
    public static string Generate(string title, IEnumerable<string> items)
    {
        var itemsList = items.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList();
        var count = itemsList.Count;
        var content = string.Join(", ", itemsList);

        return $"""


            **{count}** {title.ToLower()} supported:

            {content}


            """;
    }
}
