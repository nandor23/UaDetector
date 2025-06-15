using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UaDetector.SourceGenerator;

[Generator]
public class RegexesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        Debugger.Launch();

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
        // Only support properties with attributes
        return node is PropertyDeclarationSyntax { AttributeLists.Count: > 0 };
    }

    private static PropertyDeclarationInfo? GetSemanticTargetForGeneration(
        GeneratorSyntaxContext context
    )
    {
        if (context.Node is not PropertyDeclarationSyntax propertyDeclaration)
            return null;

        var propertyName = propertyDeclaration.Identifier.ValueText;
        var propertyAccessibility = GetPropertyAccessibility(propertyDeclaration);

        var propertySymbol =
            context.SemanticModel.GetDeclaredSymbol(propertyDeclaration) as IPropertySymbol;
        var propertyType =
            propertySymbol?.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            ?? throw new InvalidOperationException("Unable to determine property type");

        foreach (var attributeList in propertyDeclaration.AttributeLists)
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
                                var containingClass =
                                    GetContainingClassName(propertyDeclaration)
                                    ?? throw new InvalidOperationException(
                                        "Containing class not found"
                                    );
                                var namespaceName =
                                    GetNamespaceName(propertyDeclaration)
                                    ?? throw new InvalidOperationException("Namespace not found");
                                var classAccessibility = GetClassAccessibility(propertyDeclaration);
                                var isStaticClass = IsContainingClassStatic(propertyDeclaration);

                                return new PropertyDeclarationInfo
                                {
                                    PropertyName = propertyName,
                                    ResourcePath = resourcePath,
                                    ContainingClass = containingClass,
                                    Namespace = namespaceName,
                                    PropertyType = propertyType,
                                    PropertyAccessibility = propertyAccessibility,
                                    ClassAccessibility = classAccessibility,
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

    private static string GetPropertyAccessibility(PropertyDeclarationSyntax propertyDeclaration)
    {
        foreach (var modifier in propertyDeclaration.Modifiers)
        {
            return modifier.Kind() switch
            {
                SyntaxKind.PublicKeyword => "public",
                SyntaxKind.InternalKeyword => "internal",
                SyntaxKind.PrivateKeyword => "private",
                SyntaxKind.ProtectedKeyword => "protected",
                _ => "internal"
            };
        }

        return "internal";
    }

    private static string? GetContainingClassName(SyntaxNode node)
    {
        var classDeclaration = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        return classDeclaration?.Identifier.ValueText;
    }

    private static string? GetNamespaceName(SyntaxNode node)
    {
        var namespaceDeclaration = node.Ancestors()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .FirstOrDefault();
        return namespaceDeclaration?.Name.ToString();
    }

    private static string GetClassAccessibility(SyntaxNode node)
    {
        var classDeclaration = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();

        if (classDeclaration == null)
            return "internal";

        foreach (var modifier in classDeclaration.Modifiers)
        {
            return modifier.Kind() switch
            {
                SyntaxKind.PublicKeyword => "public",
                SyntaxKind.InternalKeyword => "internal",
                SyntaxKind.PrivateKeyword => "private",
                SyntaxKind.ProtectedKeyword => "protected",
                _ => "internal"
            };
        }

        return "internal";
    }

    private static bool IsContainingClassStatic(SyntaxNode node)
    {
        var classDeclaration = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();

        if (classDeclaration == null)
            return false;

        return classDeclaration.Modifiers.Any(modifier =>
            modifier.IsKind(SyntaxKind.StaticKeyword)
        );
    }

    private static void Execute(
        Compilation compilation,
        ImmutableArray<PropertyDeclarationInfo> properties,
        SourceProductionContext context
    )
    {
        if (properties.IsDefaultOrEmpty)
            return;

        foreach (var property in properties)
        {
            var sourceCode = GenerateSource(property);
            context.AddSource(
                $"{property.ContainingClass}_{property.PropertyName}.g.cs",
                sourceCode
            );
        }
    }

    private static string GenerateSource(PropertyDeclarationInfo property)
    {
        string valueExpr;
        var innerType = ExtractGenericTypeArgument(property.PropertyType);

        if (property.PropertyType.StartsWith("global::System.Collections.Generic.IReadOnlyList<"))
        {
            valueExpr = $"System.Array.Empty<{innerType}>()";
        }
        else
        {
            throw new NotSupportedException(
                $"Unsupported property type: {property.PropertyType} for property {property.PropertyName}"
            );
        }

        var staticModifier = property.IsStaticClass ? "static " : string.Empty;

        var fieldName = $"_{property.PropertyName.ToLower()}";

        return $$"""
            using System.Collections.Frozen;

            namespace {{property.Namespace}};

            {{property.ClassAccessibility}} {{staticModifier}}partial class {{property.ContainingClass}}
            {
                private readonly static {{property.PropertyType}} {{fieldName}} = {{valueExpr}};

                {{property.PropertyAccessibility}} static partial {{property.PropertyType}} {{property.PropertyName}}
                {
                    get => {{fieldName}}; 
                }
            }
            """;
    }

    private static string ExtractGenericTypeArgument(string genericType)
    {
        var startIndex = genericType.IndexOf('<') + 1;
        var endIndex = genericType.LastIndexOf('>');

        if (startIndex > 0 && startIndex < endIndex)
        {
            return genericType.Substring(startIndex, endIndex - startIndex);
        }

        throw new ArgumentException($"Cannot extract generic type argument from: {genericType}");
    }

    private sealed class PropertyDeclarationInfo
    {
        public required string PropertyName { get; init; }
        public required string ResourcePath { get; init; }
        public required string ContainingClass { get; init; }
        public required string Namespace { get; init; }
        public required string PropertyType { get; init; }
        public required string PropertyAccessibility { get; init; }
        public required string ClassAccessibility { get; init; }
        public required bool IsStaticClass { get; init; }
    }
}
