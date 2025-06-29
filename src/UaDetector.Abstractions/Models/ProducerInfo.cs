namespace UaDetector.Abstractions.Models;

public sealed class ProducerInfo
{
    public string? Name { get; init; }
    public string? Url { get; init; }

    public override string ToString()
    {
        return string.Join(
            ", ",
            new[]
            {
                string.IsNullOrEmpty(Name) ? null : $"{nameof(Name)}: {Name}",
                string.IsNullOrEmpty(Url) ? null : $"{nameof(Url)}: {Url}",
            }.Where(x => !string.IsNullOrEmpty(x))
        );
    }
}
