using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UaDetector.SourceGenerator.Models;
using UaDetector.SourceGenerator.Utilities;

namespace UaDetector.SourceGenerator;

[Generator]
internal sealed class HintSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var hintSourceProvider = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                "UaDetector.Abstractions.Attributes.HintSource",
                predicate: static (node, _) => node is PropertyDeclarationSyntax,
                transform: GetHintSourceForGeneration
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
            hintSourceProvider.Combine(additionalFiles),
            static (spc, source) => Execute(source.Left, source.Right, spc)
        );
    }

    private static HintSourceProperty? GetHintSourceForGeneration(
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
                OriginalDefinition:
                {
                    Name: "FrozenDictionary",
                    ContainingNamespace.Name: "Frozen",
                    ContainingNamespace.ContainingNamespace.Name: "Collections",
                    ContainingNamespace.ContainingNamespace.ContainingNamespace.Name: "System"
                },
                TypeArguments: [
                    INamedTypeSymbol { SpecialType: SpecialType.System_String },
                    INamedTypeSymbol { SpecialType: SpecialType.System_String },
                ]
            }
        )
        {
            return null;
        }

        var containingClass = propertySymbol.ContainingSymbol;
        var @namespace = containingClass
            .ContainingNamespace.ToString()
            .NullIf("<global namespace>");

        return new HintSourceProperty
        {
            PropertyName = propertySymbol.Name,
            ResourcePath = path,
            ContainingClass = containingClass.Name,
            Namespace = @namespace is not null ? $"namespace {@namespace};" : string.Empty,
            PropertyAccessibility = propertySymbol.DeclaredAccessibility,
            IsStaticClass = containingClass is INamedTypeSymbol { IsStatic: true },
        };
    }

    private static void Execute(
        HintSourceProperty property,
        ImmutableArray<(string Path, string? Json)> additionalFiles,
        SourceProductionContext context
    )
    {
        (_, string? json) = additionalFiles.FirstOrDefault(file =>
            file.Path.EndsWith(property.ResourcePath, StringComparison.OrdinalIgnoreCase)
        );

        if (json is not null)
        {
            var list = JsonUtils.DeserializeDictionary(json);
            var fieldName = $"_{property.PropertyName}";
            var classModifier = property.IsStaticClass ? "static partial" : "partial";

            var sb = new IndentedStringBuilder();

            sb.AppendLine(
                $$"""
                {{property.Namespace}}

                {{classModifier}} class {{property.ContainingClass}}
                {
                    private static readonly global::System.Collections.Frozen.FrozenDictionary<string, string> {{fieldName}} =
                        global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(
                            new global::System.Collections.Generic.Dictionary<string, string>
                            {
                """
            );

            foreach (var kvp in list)
            {
                sb.AppendLine($"            {{ \"{kvp.Key}\", \"{kvp.Value}\" }},");
            }

            sb.AppendLine("        });\n");

            sb.AppendLine(
                $"    {property.PropertyAccessibility.ToSyntaxString()} static partial global::System.Collections.Frozen.FrozenDictionary<string, string> {property.PropertyName} => {fieldName};"
            );
            sb.AppendLine("}");

            context.AddSource($"{property.ContainingClass}.g.cs", sb.ToString());
        }
    }
}
