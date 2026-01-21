using System.Text.RegularExpressions;

namespace UaDetector.DocsGenerator.Utilities;

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
    public static string ReplaceMarkerContent(
        this string content,
        string markerName,
        string newContent
    )
    {
        return ReplaceContent(content, markerName, newContent);
    }

    private static string ReplaceContent(this string content, string markerName, string newContent)
    {
        string sectionPattern =
            $@"(<!--\s*{Regex.Escape(markerName)}\s*-->)(.*?)(<!--\s*{Regex.Escape(markerName)}\s*-->)";
        var regex = new Regex(sectionPattern, RegexOptions.Singleline);

        if (regex.IsMatch(content))
        {
            return regex.Replace(
                content,
                m => $"{m.Groups[1].Value}{newContent}{m.Groups[3].Value}"
            );
        }

        throw new ArgumentException($"Markers '<!-- {markerName} -->' not found in content");
    }
}
