namespace UaDetector.Attributes;

[AttributeUsage(AttributeTargets.Property)]
internal sealed class RegexSourceAttribute : Attribute
{
    public string FilePath { get; }
    public string? RegexSuffix { get; }

    public RegexSourceAttribute(string filePath, string? regexSuffix = null)
    {
        FilePath = filePath;
        RegexSuffix = regexSuffix;
    }
}
