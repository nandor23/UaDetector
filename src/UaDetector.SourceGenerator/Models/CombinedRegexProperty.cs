using Microsoft.CodeAnalysis;

namespace UaDetector.SourceGenerator.Models;

public sealed record CombinedRegexProperty
{
    public required string PropertyName { get; init; }
    public required string ContainingClassFullName { get; init; }
    public required Accessibility PropertyAccessibility { get; init; }
}
