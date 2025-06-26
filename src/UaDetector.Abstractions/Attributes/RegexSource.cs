namespace UaDetector.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class RegexSource : Attribute
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
                "The file name must be a JSON file ending with '.json'.",
                nameof(filePath)
            );

        FilePath = filePath;
        RegexSuffix = regexSuffix;
    }
}
