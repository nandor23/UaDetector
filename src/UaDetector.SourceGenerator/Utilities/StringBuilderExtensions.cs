using System.Text;

namespace UaDetector.SourceGenerator.Utilities;

public static class StringBuilderExtensions
{
    public static StringBuilder AppendIndentedLine(
        this StringBuilder sb,
        int indentLevel,
        string line
    )
    {
        sb.AppendLine(new string('\t', indentLevel) + line);
        return sb;
    }
}
