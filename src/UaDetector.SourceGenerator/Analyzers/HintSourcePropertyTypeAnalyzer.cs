using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UaDetector.SourceGenerator.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class HintSourcePropertyTypeAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Descriptor =
        DiagnosticDescriptors.InvalidFrozenDictionaryPropertyType;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Descriptor];

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
                == "UaDetector.Attributes.HintSourceAttribute"
            )
            {
                var propertyType = propertySymbol.Type;

                if (
                    propertyType
                    is not INamedTypeSymbol
                    {
                        OriginalDefinition:
                        {
                            Name: "FrozenDictionary",
                            ContainingNamespace.Name: "Frozen",
                            ContainingNamespace.ContainingNamespace.Name: "Collections",
                            ContainingNamespace
                                .ContainingNamespace
                                .ContainingNamespace
                                .Name: "System"
                        },
                        TypeArguments: [
                            INamedTypeSymbol { SpecialType: SpecialType.System_String },
                            INamedTypeSymbol { SpecialType: SpecialType.System_String },
                        ]
                    }
                )
                {
                    var diagnostic = Diagnostic.Create(Descriptor, propertySymbol.Locations[0]);
                    context.ReportDiagnostic(diagnostic);
                }

                break;
            }
        }
    }
}
