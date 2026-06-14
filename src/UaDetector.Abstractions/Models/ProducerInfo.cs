namespace UaDetector.Abstractions.Models;

public sealed record ProducerInfo
{
    public string? Name { get; init; }
    public string? Url { get; init; }
}
