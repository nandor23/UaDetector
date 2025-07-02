using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UaDetector.SourceGenerator.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class HintSourcePropertyModelTypeAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Descriptor =
        DiagnosticDescriptors.InvalidIReadOnlyListModelType;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Descriptor];

    private static readonly HashSet<string> AllowedModelTypes =
    [
        "UaDetector.Models.Client",
        "UaDetector.Models.Browser",
        "UaDetector.Models.Engine",
        "UaDetector.Models.Os",
        "UaDetector.Models.Device",
        "UaDetector.Models.Bot",
        "UaDetector.Models.VendorFragment",
    ];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeProperty, SymbolKind.Property);
    }

    private static void AnalyzeProperty(SymbolAnalysisContext context)
    {
        var propertySymbol = (IPropertySymbol)context.Symbol;

        foreach (var attribute in propertySymbol.GetAttributes())
        {
            if (
                attribute.AttributeClass?.ToDisplayString()
                == "UaDetector.Attributes.RegexSourceAttribute"
            )
            {
                var propertyType = propertySymbol.Type;

                if (
                    propertyType
                    is not INamedTypeSymbol
                    {
                        OriginalDefinition.SpecialType: SpecialType.System_Collections_Generic_IReadOnlyList_T,
                        TypeArguments: [INamedTypeSymbol elementType],
                    }
                )
                {
                    Report(context, propertySymbol);
                    break;
                }

                var modelTypeName = elementType.ToDisplayString();

                if (!AllowedModelTypes.Contains(modelTypeName))
                {
                    Report(context, propertySymbol);
                }

                break;
            }
        }
    }

    private static void Report(SymbolAnalysisContext context, IPropertySymbol property)
    {
        var diagnostic = Diagnostic.Create(Descriptor, property.Locations[0]);
        context.ReportDiagnostic(diagnostic);
    }
}
