namespace UaDetector.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class RegexesAttribute : Attribute
{
    public string ResourcePath { get; }

    public RegexesAttribute(string resourcePath)
    {
        if (
            string.IsNullOrWhiteSpace(resourcePath)
            || !resourcePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
        )
            throw new ArgumentException(
                "The file name must be a JSON file ending with '.json'.",
                nameof(resourcePath)
            );

        ResourcePath = resourcePath;
    }
}
