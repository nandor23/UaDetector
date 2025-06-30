namespace UaDetector.Attributes;

[AttributeUsage(AttributeTargets.Property)]
internal sealed class HintSource : Attribute
{
    public string FilePath { get; }

    public HintSource(string filePath)
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
    }
}
