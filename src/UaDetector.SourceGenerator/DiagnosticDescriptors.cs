using Microsoft.CodeAnalysis;

namespace UaDetector.SourceGenerator;

internal static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor JsonDeserializeFailed = new(
        id: "UAD001",
        title: "JSON Deserialization Failed",
        messageFormat: "Failed to deserialize JSON from '{0}'",
        category: "UaDetector.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    internal static readonly DiagnosticDescriptor InvalidFrozenDictionaryPropertyType = new(
        id: "UAD002",
        title: "Invalid FrozenDictionary Property Type",
        messageFormat: "Property must be of type FrozenDictionary<string, string>",
        category: "UaDetector.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
}
