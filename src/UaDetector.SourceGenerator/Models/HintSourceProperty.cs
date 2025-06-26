using Microsoft.CodeAnalysis;

namespace UaDetector.SourceGenerator.Models;

internal sealed record HintSourceProperty
{
    public required string PropertyName { get; init; }
    public required string ResourcePath { get; init; }
    public required string ContainingClass { get; init; }
    public required string Namespace { get; init; }
    public required Accessibility PropertyAccessibility { get; init; }
    public required bool IsStaticClass { get; init; }
}
