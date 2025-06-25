using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UaDetector.Abstractions;
using UaDetector.Abstractions.Models;
using UaDetector.Abstractions.Models.Browsers;
using UaDetector.SourceGenerator.Generators;
using UaDetector.SourceGenerator.Models;
using UaDetector.SourceGenerator.Utilities;
using Engine = UaDetector.Abstractions.Models.Browsers.Engine;

namespace UaDetector.SourceGenerator;

[Generator]
internal sealed class RegexGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var regexSourceProvider = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                "UaDetector.Abstractions.Attributes.RegexSource",
                predicate: static (node, _) => node is PropertyDeclarationSyntax,
                transform: GetRegexSourceForGeneration
            )
            .Where(static m => m is not null)
            .Select(static (m, _) => m!);

        var combinedRegexProvider = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                "UaDetector.Abstractions.Attributes.CombinedRegex",
                predicate: static (node, _) => node is PropertyDeclarationSyntax,
                transform: GetCombinedRegexForGeneration
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
            regexSourceProvider.Combine(combinedRegexProvider.Collect()).Combine(additionalFiles),
            static (spc, source) => Execute(source.Left.Left, source.Left.Right, source.Right, spc)
        );
    }

    private static RegexSourceProperty? GetRegexSourceForGeneration(
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
        {
            return null;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var propertySymbol = (IPropertySymbol)context.TargetSymbol;
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
            return null;
        }

        var containingClass = propertySymbol.ContainingSymbol;
        var @namespace = containingClass
            .ContainingNamespace.ToString()
            .NullIf("<global namespace>");

        return new RegexSourceProperty
        {
            PropertyName = propertySymbol.Name,
            ResourcePath = path,
            ContainingClass = containingClass.Name,
            ContainingClassFullName = containingClass.ToDisplayString(
                SymbolDisplayFormat.FullyQualifiedFormat
            ),
            Namespace = @namespace is not null ? $"namespace {@namespace};" : string.Empty,
            ElementType = elementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            PropertyAccessibility = propertySymbol.DeclaredAccessibility,
            IsStaticClass = containingClass is INamedTypeSymbol { IsStatic: true },
        };
    }

    private static CombinedRegexProperty? GetCombinedRegexForGeneration(
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var propertySymbol = (IPropertySymbol)context.TargetSymbol;
        var containingClass = propertySymbol.ContainingSymbol;

        if (
            propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            != "global::System.Text.RegularExpressions.Regex"
        )
        {
            return null;
        }

        return new CombinedRegexProperty
        {
            PropertyName = propertySymbol.Name,
            ContainingClassFullName = containingClass.ToDisplayString(
                SymbolDisplayFormat.FullyQualifiedFormat
            ),
            PropertyAccessibility = propertySymbol.DeclaredAccessibility,
        };
    }

    private static void Execute(
        RegexSourceProperty regexSourceProperty,
        ImmutableArray<CombinedRegexProperty> combinedRegexProperties,
        ImmutableArray<(string Path, string? Json)> additionalFiles,
        SourceProductionContext context
    )
    {
        (_, string? json) = additionalFiles.FirstOrDefault(file =>
            file.Path.EndsWith(regexSourceProperty.ResourcePath, StringComparison.OrdinalIgnoreCase)
        );

        if (json is not null)
        {
            var usageCount = combinedRegexProperties.Count(r =>
                r.ContainingClassFullName == regexSourceProperty.ContainingClassFullName
            );

            if (usageCount > 1)
            {
                return;
            }

            var combinedRegexProperty = combinedRegexProperties.FirstOrDefault(p =>
                p.ContainingClassFullName == regexSourceProperty.ContainingClassFullName
            );

            var sourceCode = GenerateSource(json, regexSourceProperty, combinedRegexProperty);

            context.AddSource($"{regexSourceProperty.ContainingClass}.g.cs", sourceCode);
        }
    }

    private static string GenerateSource(
        string json,
        RegexSourceProperty regexSourceProperty,
        CombinedRegexProperty? combinedRegexProperty
    )
    {
        if (regexSourceProperty.ElementType is null)
        {
            throw new NotSupportedException();
        }

        if (regexSourceProperty.ElementType == GetGlobalQualifiedName<Client>())
        {
            return ClientSourceGenerator.Generate(json, regexSourceProperty, combinedRegexProperty);
        }

        if (regexSourceProperty.ElementType == GetGlobalQualifiedName<Device>())
        {
            return DeviceGenerator.Generate(json, regexSourceProperty, combinedRegexProperty);
        }

        if (regexSourceProperty.ElementType == GetGlobalQualifiedName<Browser>())
        {
            return BrowserSourceGenerator.Generate(
                json,
                regexSourceProperty,
                combinedRegexProperty
            );
        }

        if (regexSourceProperty.ElementType == GetGlobalQualifiedName<Engine>())
        {
            return EngineSourceGenerator.Generate(json, regexSourceProperty, combinedRegexProperty);
        }

        if (regexSourceProperty.ElementType == GetGlobalQualifiedName<Os>())
        {
            return OsSourceGenerator.Generate(json, regexSourceProperty, combinedRegexProperty);
        }

        if (regexSourceProperty.ElementType == GetGlobalQualifiedName<Bot>())
        {
            return BotSourceGenerator.Generate(json, regexSourceProperty, combinedRegexProperty);
        }

        throw new NotSupportedException();
    }

    private static string GetGlobalQualifiedName<T>()
    {
        return $"global::{typeof(T).FullName}";
    }
}
