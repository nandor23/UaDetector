namespace UaDetector.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class RegexesAttribute : Attribute
{
    public string FilePath { get; }

    public RegexesAttribute(string filePath)
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
