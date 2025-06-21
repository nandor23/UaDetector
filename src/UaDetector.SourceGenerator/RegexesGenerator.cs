using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UaDetector.Models.Browsers;
using UaDetector.SourceGenerator.Generators;
using UaDetector.SourceGenerator.Models;
using UaDetector.SourceGenerator.Utilities;

namespace UaDetector.SourceGenerator;

[Generator]
public class RegexesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                "UaDetector.Attributes.RegexDefinitionsAttribute",
                predicate: static (node, _) => node is PropertyDeclarationSyntax,
                transform: GetSemanticTargetForGeneration
            )
            .Where(static m => m is not null)
            .Select(static (m, _) => m!);

        var additionalFiles = context
            .AdditionalTextsProvider.Where(file =>
                file.Path.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            )
            .Select(
                (file, ct) =>
                {
                    var path = file.Path.Replace('\\', '/');
                    var json = file.GetText(ct)?.ToString();
                    return (path, json);
                }
            )
            .Collect();

        context.RegisterSourceOutput(
            provider.Combine(additionalFiles),
            static (spc, source) => Execute(source.Left, source.Right, spc)
        );
    }

    private static PropertyDeclarationInfo? GetSemanticTargetForGeneration(
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var attribute = context.Attributes[0];
        var path = attribute
            .ConstructorArguments.FirstOrDefault()
            .Value?.ToString()
            ?.Replace('\\', '/');

        if (path is null)
            return null;

        cancellationToken.ThrowIfCancellationRequested();

        var propertySymbol = (IPropertySymbol)context.TargetSymbol;
        var propertyType = propertySymbol.Type;

        if (
            propertyType
            is not INamedTypeSymbol
            {
                OriginalDefinition.SpecialType: SpecialType.System_Collections_Generic_IReadOnlyList_T,
                TypeArguments: [{ } elementType],
            }
        )
        {
            return null;
        }

        var containingClass = propertySymbol.ContainingSymbol;
        var @namespace = containingClass
            .ContainingNamespace.ToString()
            .NullIf("<global namespace>");

        if (
            elementType
            is not INamedTypeSymbol
            {
                IsGenericType: true,
                TypeArguments.Length: > 0
            } namedElementType
        )
        {
            throw new NotSupportedException(
                $"Element type must be a generic type with one type argument: {elementType}"
            );
        }

        return new PropertyDeclarationInfo
        {
            PropertyName = propertySymbol.Name,
            ResourcePath = path,
            ContainingClass = containingClass.Name,
            Namespace = @namespace is not null ? $"namespace {@namespace};" : string.Empty,
            ElementType = elementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            ElementGenericType = namedElementType
                .TypeArguments[0]
                .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            PropertyAccessibility = propertySymbol.DeclaredAccessibility,
        };
    }

    private static void Execute(
        PropertyDeclarationInfo property,
        ImmutableArray<(string Path, string? Json)> additionalFiles,
        SourceProductionContext context
    )
    {
        (_, string? json) = additionalFiles.FirstOrDefault(file =>
            file.Path.EndsWith(property.ResourcePath, StringComparison.OrdinalIgnoreCase)
        );

        if (json is not null)
        {
            var sourceCode = GenerateSource(property, json);

            context.AddSource(
                $"{property.ContainingClass}_{property.PropertyName}.g.cs",
                sourceCode
            );
        }
    }

    private static string GenerateSource(PropertyDeclarationInfo property, string json)
    {
        if (property.ElementGenericType is null)
        {
            throw new NotSupportedException();
        }

        if (property.ElementGenericType == GetGlobalQualifiedName<Browser>())
        {
            return BrowserSourceGenerator.Generate(property, json);
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    private static string GetGlobalQualifiedName<T>()
    {
        return $"global::{typeof(T).FullName}";
    }
}
