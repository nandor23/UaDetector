using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UaDetector.SourceGenerator;

[Generator]
public class RegexesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context
            .SyntaxProvider.CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx)
            )
            .Where(static m => m is not null)
            .Select(static (m, _) => m!);

        var compilation = context.CompilationProvider.Combine(provider.Collect());

        context.RegisterSourceOutput(
            compilation,
            static (spc, source) => Execute(source.Left, source.Right, spc)
        );
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        return node is FieldDeclarationSyntax { AttributeLists.Count: > 0 };
    }

    private static FieldDeclarationInfo? GetSemanticTargetForGeneration(
        GeneratorSyntaxContext context
    )
    {
        var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

        foreach (var attributeList in fieldDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var symbolInfo = context.SemanticModel.GetSymbolInfo(attribute);
                if (symbolInfo.Symbol is IMethodSymbol attributeSymbol)
                {
                    var attributeName = attributeSymbol.ContainingType.Name;
                    if (attributeName is "RegexesAttribute" or "Regexes")
                    {
                        if (attribute.ArgumentList?.Arguments.Count > 0)
                        {
                            var firstArgument = attribute.ArgumentList.Arguments[0];
                            if (
                                firstArgument.Expression
                                    is LiteralExpressionSyntax literalExpression
                                && literalExpression.Token.IsKind(SyntaxKind.StringLiteralToken)
                            )
                            {
                                var resourcePath = literalExpression.Token.ValueText;
                                var variable = fieldDeclaration.Declaration.Variables.First();
                                var fieldName = variable.Identifier.ValueText;
                                var containingClass = GetContainingClassName(fieldDeclaration);
                                var namespaceName = GetNamespaceName(fieldDeclaration);

                                var fieldSymbol =
                                    context.SemanticModel.GetDeclaredSymbol(variable)
                                    as IFieldSymbol;
                                var fieldType =
                                    fieldSymbol?.Type.ToDisplayString(
                                        SymbolDisplayFormat.FullyQualifiedFormat
                                    ) ?? "unknown";

                                return new FieldDeclarationInfo
                                {
                                    FieldName = fieldName,
                                    ResourcePath = resourcePath,
                                    ContainingClass = containingClass,
                                    Namespace = namespaceName,
                                    FieldType = fieldType,
                                };
                            }
                        }
                    }
                }
            }
        }

        return null;
    }

    private static string GetContainingClassName(FieldDeclarationSyntax fieldDeclaration)
    {
        var classDeclaration = fieldDeclaration
            .Ancestors()
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault();
        return classDeclaration?.Identifier.ValueText ?? "UnknownClass";
    }

    private static string GetNamespaceName(FieldDeclarationSyntax fieldDeclaration)
    {
        var namespaceDeclaration = fieldDeclaration
            .Ancestors()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .FirstOrDefault();
        return namespaceDeclaration?.Name.ToString() ?? "Global";
    }

    private static void Execute(
        Compilation compilation,
        ImmutableArray<FieldDeclarationInfo> fields,
        SourceProductionContext context
    )
    {
        if (fields.IsDefaultOrEmpty)
            return;

        foreach (var field in fields)
        {
            var sourceCode = GenerateSource(field);
            context.AddSource($"{field.ContainingClass}_{field.FieldName}.g.cs", sourceCode);
        }
    }

    private static string GenerateSource(FieldDeclarationInfo field)
    {
        var collectionKind = GetCollectionKind(field.FieldType);

        var valueExpr = collectionKind switch
        {
            CollectionKind.FrozenDictionary => "FrozenDictionary<string, string>.Empty",
            CollectionKind.List => "new List<string>()",
            _ => "throw new NotSupportedException()",
        };

        return $$"""
                 using System.Collections.Frozen;

                 namespace {{field.Namespace}};

                 internal static partial class {{field.ContainingClass}}
                 {
                     static {{field.ContainingClass}}()
                     {
                         {{field.FieldName}} = {{valueExpr}};
                     }
                 }
                 """;
    }

    private static CollectionKind GetCollectionKind(string fieldType)
    {
        if (fieldType.StartsWith("global::System.Collections.Frozen.FrozenDictionary<"))
            return CollectionKind.FrozenDictionary;

        if (fieldType.StartsWith("global::System.Collections.Generic.List<"))
            return CollectionKind.List;

        return CollectionKind.Unknown;
    }

    private enum CollectionKind
    {
        FrozenDictionary,
        List,
        Unknown,
    }

    private sealed class FieldDeclarationInfo
    {
        public required string FieldName { get; init; }
        public required string ResourcePath { get; init; }
        public required string ContainingClass { get; init; }
        public required string Namespace { get; init; }
        public required string FieldType { get; init; }
    }
}
