using System.Text.RegularExpressions;

namespace UaDetector.ReadmeUpdater;

public static class StringExtensions
{
    /// <summary>
    /// Replaces all content between matching HTML comment marker pairs while preserving the markers.
    /// <example>
    /// For marker "VERSION":
    /// <code>
    /// &lt;!-- VERSION --&gt;1.0&lt;!-- VERSION --&gt;
    /// becomes
    /// &lt;!-- VERSION --&gt;2.0&lt;!-- VERSION --&gt;
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="content">The string to modify</param>
    /// <param name="markerName">Name between comment markers (e.g. "VERSION")</param>
    /// <param name="newContent">Content to place between the markers</param>
    /// <exception cref="ArgumentException">Thrown when the specified markers are not found in the content</exception>
    public static string ReplaceBetweenMarkers(this string content, string markerName, string newContent)
    {
        return ReplaceContent(content, markerName, newContent);
    }

    /// <summary>
    /// Replaces all content between matching HTML comment marker pairs with joined enumerable content.
    /// <example>
    /// For marker "BROWSERS":
    /// <code>
    /// &lt;!-- BROWSERS --&gt;Chrome, Firefox&lt;!-- BROWSERS --&gt;
    /// becomes
    /// &lt;!-- BROWSERS --&gt;Edge, Safari, Chrome&lt;!-- BROWSERS --&gt;
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="content">The string to modify</param>
    /// <param name="markerName">Name between comment markers (e.g. "BROWSERS")</param>
    /// <param name="newContentItems">Items to join and place between the markers</param>
    /// <exception cref="ArgumentException">Thrown when the specified markers are not found in the content</exception>
    public static string ReplaceBetweenMarkers(
        this string content,
        string markerName,
        IEnumerable<string> newContentItems
    )
    {
        return ReplaceContent(
            content,
            markerName,
            string.Join(", ", newContentItems.Order(StringComparer.OrdinalIgnoreCase))
        );
    }

    private static string ReplaceContent(this string content, string markerName, string newContent)
    {
        string sectionPattern =
            $@"(<!--\s*{Regex.Escape(markerName)}\s*-->)(.*?)(<!--\s*{Regex.Escape(markerName)}\s*-->)";
        var regex = new Regex(sectionPattern, RegexOptions.Singleline);

        if (regex.IsMatch(content))
        {
            return regex.Replace(content,
                m => $"{m.Groups[1].Value}{newContent}{m.Groups[3].Value}");
        }

        throw new ArgumentException($"Markers '<!-- {markerName} -->' not found in content");
    }
}
