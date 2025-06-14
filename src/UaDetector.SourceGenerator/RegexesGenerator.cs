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
                                var containingClass =
                                    GetContainingClassName(fieldDeclaration)
                                    ?? throw new InvalidOperationException(
                                        "Containing class not found"
                                    );
                                var namespaceName =
                                    GetNamespaceName(fieldDeclaration)
                                    ?? throw new InvalidOperationException("Namespace not found");
                                var isStaticClass = IsContainingClassStatic(fieldDeclaration);

                                var fieldSymbol =
                                    context.SemanticModel.GetDeclaredSymbol(variable)
                                    as IFieldSymbol;
                                var fieldType =
                                    fieldSymbol?.Type.ToDisplayString(
                                        SymbolDisplayFormat.FullyQualifiedFormat
                                    )
                                    ?? throw new InvalidOperationException(
                                        "Unable to determine field type"
                                    );

                                return new FieldDeclarationInfo
                                {
                                    FieldName = fieldName,
                                    ResourcePath = resourcePath,
                                    ContainingClass = containingClass,
                                    Namespace = namespaceName,
                                    FieldType = fieldType,
                                    IsStaticClass = isStaticClass,
                                };
                            }
                        }
                    }
                }
            }
        }

        return null;
    }

    private static string? GetContainingClassName(FieldDeclarationSyntax fieldDeclaration)
    {
        var classDeclaration = fieldDeclaration
            .Ancestors()
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault();
        return classDeclaration?.Identifier.ValueText;
    }

    private static string? GetNamespaceName(FieldDeclarationSyntax fieldDeclaration)
    {
        var namespaceDeclaration = fieldDeclaration
            .Ancestors()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .FirstOrDefault();
        return namespaceDeclaration?.Name.ToString();
    }

    private static bool IsContainingClassStatic(FieldDeclarationSyntax fieldDeclaration)
    {
        var classDeclaration = fieldDeclaration
            .Ancestors()
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault();

        if (classDeclaration == null)
            return false;

        return classDeclaration.Modifiers.Any(modifier =>
            modifier.IsKind(SyntaxKind.StaticKeyword)
        );
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
        string valueExpr;

        if (field.FieldType.StartsWith("global::System.Collections.Frozen.FrozenDictionary<"))
        {
            valueExpr = "FrozenDictionary<string, string>.Empty";
        }
        else if (field.FieldType.StartsWith("global::System.Collections.Generic.IReadOnlyList<"))
        {
            valueExpr = "Array.Empty<string>()";
        }
        else
        {
            throw new NotSupportedException(
                $"Unsupported field type: {field.FieldType} for field {field.FieldName}"
            );
        }

        var classModifiers = field.IsStaticClass ? "static " : String.Empty;

        return $$"""
            using System.Collections.Frozen;

            namespace {{field.Namespace}};

            internal {{classModifiers}}partial class {{field.ContainingClass}}
            {
                static {{field.ContainingClass}}()
                {
                    {{field.FieldName}} = {{valueExpr}};
                }
            }
            """;
    }

    private sealed class FieldDeclarationInfo
    {
        public required string FieldName { get; init; }
        public required string ResourcePath { get; init; }
        public required string ContainingClass { get; init; }
        public required string Namespace { get; init; }
        public required string FieldType { get; init; }
        public required bool IsStaticClass { get; init; }
    }
}
