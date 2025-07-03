namespace UaDetector.Attributes;

[AttributeUsage(AttributeTargets.Property)]
internal sealed class HintSourceAttribute : Attribute
{
    public string FilePath { get; }

    public HintSourceAttribute(string filePath)
    {
        FilePath = filePath;
    }
}
