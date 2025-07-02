using Microsoft.CodeAnalysis;

namespace UaDetector.SourceGenerator;

internal static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor JsonDeserializationFailed = new(
        id: "UAD001",
        title: "Invalid JSON Format",
        messageFormat: "Unable to deserialize JSON content",
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

    internal static readonly DiagnosticDescriptor InvalidIReadOnlyListModelType = new(
        id: "UAD003",
        title: "Invalid IReadOnlyList Model Type",
        messageFormat: "Property must be of type IReadOnlyList<T> where T is one of: Client, Browser, Engine, Os, Device, Bot, VendorFragment",
        category: "UaDetector.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
}
