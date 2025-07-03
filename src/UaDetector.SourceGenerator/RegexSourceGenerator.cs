using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UaDetector.SourceGenerator.Generators;
using UaDetector.SourceGenerator.Models;
using UaDetector.SourceGenerator.Utilities;

namespace UaDetector.SourceGenerator;

[Generator]
public sealed class RegexSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var regexSourceProvider = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                "UaDetector.Attributes.RegexSourceAttribute",
                predicate: static (node, _) => node is PropertyDeclarationSyntax,
                transform: GetRegexSourceForGeneration
            )
            .Where(static m => m is not null)
            .Select(static (m, _) => m!);

        var combinedRegexProvider = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                "UaDetector.Attributes.CombinedRegexAttribute",
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

        string? regexSuffix =
            attribute.ConstructorArguments.Length > 1
                ? attribute.ConstructorArguments[1].Value?.ToString()
                : null;

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
            RegexSuffix = regexSuffix,
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
            var combinedRegexProperty = combinedRegexProperties.FirstOrDefault(p =>
                p.ContainingClassFullName == regexSourceProperty.ContainingClassFullName
            );

            if (
                !TryGenerateSource(
                    json,
                    regexSourceProperty,
                    combinedRegexProperty,
                    out var sourceCode
                )
            )
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.JsonDeserializationFailed,
                    Location.None
                );

                context.ReportDiagnostic(diagnostic);
                return;
            }

            context.AddSource($"{regexSourceProperty.ContainingClass}.g.cs", sourceCode);
        }
    }

    private static bool TryGenerateSource(
        string json,
        RegexSourceProperty regexSourceProperty,
        CombinedRegexProperty? combinedRegexProperty,
        [NotNullWhen(true)] out string? result
    )
    {
        return regexSourceProperty.ElementType switch
        {
            "global::UaDetector.Models.Client" => ClientSourceGenerator.TryGenerate(
                json,
                regexSourceProperty,
                combinedRegexProperty,
                out result
            ),
            "global::UaDetector.Models.Browser" => BrowserSourceGenerator.TryGenerate(
                json,
                regexSourceProperty,
                combinedRegexProperty,
                out result
            ),
            "global::UaDetector.Models.Engine" => EngineSourceGenerator.TryGenerate(
                json,
                regexSourceProperty,
                combinedRegexProperty,
                out result
            ),
            "global::UaDetector.Models.Os" => OsSourceGenerator.Generate(
                json,
                regexSourceProperty,
                combinedRegexProperty,
                out result
            ),
            "global::UaDetector.Models.Device" => DeviceGenerator.TryGenerate(
                json,
                regexSourceProperty,
                combinedRegexProperty,
                out result
            ),
            "global::UaDetector.Models.Bot" => BotSourceGenerator.TryGenerate(
                json,
                regexSourceProperty,
                combinedRegexProperty,
                out result
            ),
            "global::UaDetector.Models.VendorFragment" => VendorFragmentSourceGenerator.Generate(
                json,
                regexSourceProperty,
                combinedRegexProperty,
                out result
            ),
            _ => throw new NotSupportedException(
                $"Unsupported ElementType: {regexSourceProperty.ElementType}"
            ),
        };
    }
}
