using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using UaDetector.SourceGenerator.Utilities;

namespace UaDetector.SourceGenerator.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class CombinedRegexWithoutRegexSourceAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Descriptor =
        DiagnosticDescriptors.CombinedRegexWithoutRegexSource;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Descriptor];

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
            var diagnostic = Diagnostic.Create(Descriptor, propertySymbol.Locations[0]);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool HasAttribute(ISymbol symbol, string fullAttributeName)
    {
        return symbol
            .GetAttributes()
            .Any(attr => attr.AttributeClass?.ToDisplayString() == fullAttributeName);
    }
}
