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

    public static readonly DiagnosticDescriptor InvalidFrozenDictionaryPropertyType = new(
        id: "UAD002",
        title: "Invalid FrozenDictionary Property Type",
        messageFormat: "Property must be of type FrozenDictionary<string, string>",
        category: "UaDetector.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static readonly DiagnosticDescriptor InvalidIReadOnlyListModelType = new(
        id: "UAD003",
        title: "Invalid IReadOnlyList Model Type",
        messageFormat: "Property must be of type IReadOnlyList<T> where T is one of: Client, Browser, Engine, Os, Device, Bot, VendorFragment",
        category: "UaDetector.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static readonly DiagnosticDescriptor CombinedRegexWithoutRegexSource = new(
        id: "UAD004",
        title: "CombinedRegexAttribute requires RegexSourceAttribute in same file",
        messageFormat: "[CombinedRegex] cannot be used unless a [RegexSource] exists in the same file",
        category: "UaDetector.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static readonly DiagnosticDescriptor CombinedRegexInvalidType = new(
        id: "UAD005",
        title: "CombinedRegexAttribute must be applied to a property of type Regex",
        messageFormat: "[CombinedRegex] can only be used on properties of type Regex",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
}
