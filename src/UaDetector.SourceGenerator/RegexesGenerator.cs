using System.Collections.Immutable;
using System.Text.Json;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using UaDetector.Models.Browsers;

namespace UaDetector.SourceGenerator;

[Generator]
public class RegexesGenerator : IIncrementalGenerator
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "UaDetector.Attributes.RegexesAttribute",
                predicate: static (node, _) => node is PropertyDeclarationSyntax,
                transform: GetSemanticTargetForGeneration
            )
            .Where(static m => m is not null)
            .Select(static (m, _) => m!);

        var additionalFiles = context.AdditionalTextsProvider
            .Where(file =>
                file.Path.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            )
            .Select((file, ct) =>
            {
                // TODO: handle the deserialization for each file separately
                var path = file.Path.Replace('\\', '/');
                var json = file.GetText(ct)?.ToString();

                if (json is not null)
                {
                    try
                    {
                        var list = JsonSerializer.Deserialize<List<BrowserRegex>>(json, SerializerOptions);
                        return (path, List: list!.ToEquatableReadOnlyList());
                    }
                    catch { }
                }

                return (path, []);
            })
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
        var path = attribute.ConstructorArguments.FirstOrDefault().Value?.ToString()?.Replace('\\', '/');

        if (path is null)
            return null;

        cancellationToken.ThrowIfCancellationRequested();

        var propertySymbol = (IPropertySymbol)context.TargetSymbol;
        var propertyType = propertySymbol.Type;

        if (propertyType is not INamedTypeSymbol
            {
                OriginalDefinition.SpecialType: SpecialType.System_Collections_Generic_IReadOnlyList_T,
                TypeArguments: [{ } elementType],
            })
        {
            return null;
        }

        var containingClass = propertySymbol.ContainingSymbol;
        var @namespace = containingClass.ContainingNamespace.ToString().NullIf("<global namespace>");

        return new PropertyDeclarationInfo
        {
            PropertyName = propertySymbol.Name,
            ResourcePath = path,
            ContainingClass = containingClass.Name,
            Namespace = @namespace != null ? $"namespace {@namespace};" : "",
            ElementType = elementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            PropertyAccessibility = propertySymbol.DeclaredAccessibility,
        };
    }

    private static void Execute(
        PropertyDeclarationInfo property,
        ImmutableArray<(string Path, EquatableReadOnlyList<BrowserRegex> List)> additionalFiles,
        SourceProductionContext context
    )
    {
        var (_, list) = additionalFiles.FirstOrDefault(file =>
            file.Path.EndsWith(property.ResourcePath, StringComparison.OrdinalIgnoreCase)
        );

        if (list.Count > 0)
        {
            var sourceCode = GenerateSource(property, list);

            context.AddSource(
                $"{property.ContainingClass}_{property.PropertyName}.g.cs",
                sourceCode
            );
        }
    }

    private static string GenerateSource(
        PropertyDeclarationInfo property,
        EquatableReadOnlyList<BrowserRegex> list
    )
    {
        // TODO: convert `list` to a C# string
        var valueExpr = "[]";
        var fieldName = $"_{property.PropertyName}";

        return $$"""
            {{property.Namespace}}

            partial class {{property.ContainingClass}}
            {
                private static readonly global::System.Collections.Generic.IReadOnlyList<{{property.ElementType}}> {{fieldName}} = {{valueExpr}};

                {{property.PropertyAccessibility.ToSyntaxString()}} static partial global::System.Collections.Generic.IReadOnlyList <{{property.ElementType}}> {{property.PropertyName}} =>
                    {{fieldName}}; 
            }
            """;
    }

    private sealed class PropertyDeclarationInfo
    {
        public required string PropertyName { get; init; }
        public required string ResourcePath { get; init; }
        public required string ContainingClass { get; init; }
        public required string? Namespace { get; init; }
        public required string ElementType { get; init; }
        public required Accessibility PropertyAccessibility { get; init; }
    }
}

file static class Extensions
{
    public static string? NullIf(this string value, string check) =>
        value.Equals(check, StringComparison.Ordinal) ? null : value;

    public static string ToSyntaxString(this Accessibility accessibility) =>
        accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Private => "private",
            Accessibility.Internal => "internal",
            Accessibility.Protected => "protected",
            _ => throw new NotSupportedException(),
        };
}
