namespace UaDetector.Attributes;

[AttributeUsage(AttributeTargets.Property)]
internal sealed class RegexSource : Attribute
{
    public string FilePath { get; }
    public string? RegexSuffix { get; }

    public RegexSource(string filePath, string? regexSuffix = null)
    {
        if (
            string.IsNullOrWhiteSpace(filePath)
            || !filePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
        )
            throw new ArgumentException(
                "The file path must refer to a valid JSON file ending with '.json'.",
                nameof(filePath)
            );

        FilePath = filePath;
        RegexSuffix = regexSuffix;
    }
}
