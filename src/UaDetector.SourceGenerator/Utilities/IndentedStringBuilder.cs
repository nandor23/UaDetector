using System.Text;

namespace UaDetector.SourceGenerator.Utilities;

internal class IndentedStringBuilder
{
    private readonly StringBuilder _sb = new();
    private int _indentLevel;

    public void Indent() => _indentLevel++;
    public void Unindent() => _indentLevel = Math.Max(0, _indentLevel - 1);

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
