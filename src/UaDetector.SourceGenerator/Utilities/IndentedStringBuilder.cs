using System.Text;

namespace UaDetector.SourceGenerator.Utilities;

internal sealed class IndentedStringBuilder
{
    private readonly StringBuilder _sb = new();
    private int _indentLevel;

    public IndentedStringBuilder Indent()
    {
        _indentLevel++;
        return this;
    }

    public IndentedStringBuilder Unindent()
    {
        _indentLevel = Math.Max(0, _indentLevel - 1);
        return this;
    }

    public IndentedStringBuilder AppendLine(string line)
    {
        if (_indentLevel > 0)
        {
            _sb.Append(new string('\t', _indentLevel));
        }

        _sb.AppendLine(line);
        return this;
    }

    public override string ToString() => _sb.ToString();
}
