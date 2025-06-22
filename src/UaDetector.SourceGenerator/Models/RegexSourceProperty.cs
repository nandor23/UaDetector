using Microsoft.CodeAnalysis;

namespace UaDetector.SourceGenerator.Models;

internal sealed class RegexSourceProperty
{
    public required string PropertyName { get; init; }
    public required string ResourcePath { get; init; }
    public required string ContainingClass { get; init; }
    public required string? Namespace { get; init; }
    public required string ElementType { get; init; }
    public required string ElementGenericType { get; init; }
    public required Accessibility PropertyAccessibility { get; init; }
}
