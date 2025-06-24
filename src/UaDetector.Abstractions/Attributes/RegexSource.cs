namespace UaDetector.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class RegexSource : Attribute
{
    public string FilePath { get; }

    public RegexSource(string filePath)
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
    }
}
