using Microsoft.CodeAnalysis;

namespace UaDetector.SourceGenerator.Models;

internal sealed record CombinedRegexProperty
{
    public required string PropertyName { get; init; }
    public required string ContainingClassFullName { get; init; }
    public required Accessibility PropertyAccessibility { get; init; }
}
