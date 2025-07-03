using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using UaDetector.SourceGenerator.Utilities;

namespace UaDetector.SourceGenerator.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class CombinedRegexAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor MissingRegexSourceDescriptor =
        DiagnosticDescriptors.CombinedRegexWithoutRegexSource;

    private static readonly DiagnosticDescriptor InvalidTypeDescriptor =
        DiagnosticDescriptors.CombinedRegexInvalidType;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [MissingRegexSourceDescriptor, InvalidTypeDescriptor];

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterCompilationStartAction(static compilationContext =>
        {
            compilationContext.RegisterSymbolAction(AnalyzeProperty, SymbolKind.Property);
        });
    }

    private static void AnalyzeProperty(SymbolAnalysisContext context)
    {
        var propertySymbol = (IPropertySymbol)context.Symbol;

        if (!HasAttribute(propertySymbol, "UaDetector.Attributes.CombinedRegexAttribute"))
            return;

        var propertyTypeName = propertySymbol.Type.ToDisplayString(
            SymbolDisplayFormat.FullyQualifiedFormat
        );
        if (propertyTypeName != "global::System.Text.RegularExpressions.Regex")
        {
            var typeDiagnostic = Diagnostic.Create(
                InvalidTypeDescriptor,
                propertySymbol.Locations[0]
            );
            context.ReportDiagnostic(typeDiagnostic);
            return;
        }

        var syntaxRef = propertySymbol.DeclaringSyntaxReferences.FirstOrDefault();
        if (syntaxRef == null)
            return;

        var syntaxTree = syntaxRef.SyntaxTree;

        var allPropertiesInTree = context
            .Compilation.GlobalNamespace.GetMembersRecursively()
            .OfType<IPropertySymbol>()
            .Where(p =>
                p.DeclaringSyntaxReferences.Any(r => r.SyntaxTree == syntaxTree)
                && HasAttribute(p, "UaDetector.Attributes.RegexSourceAttribute")
            );

        if (!allPropertiesInTree.Any())
        {
            var missingSourceDiagnostic = Diagnostic.Create(
                MissingRegexSourceDescriptor,
                propertySymbol.Locations[0]
            );
            context.ReportDiagnostic(missingSourceDiagnostic);
        }
    }

    private static bool HasAttribute(ISymbol symbol, string fullAttributeName)
    {
        return symbol
            .GetAttributes()
            .Any(attr => attr.AttributeClass?.ToDisplayString() == fullAttributeName);
    }
}
