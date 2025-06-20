using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UaDetector.Models;
using UaDetector.Models.Browsers;

namespace UaDetector.SourceGenerator;

[Generator]
public class RegexesGenerator : IIncrementalGenerator
{
    private const string RegexMethodPrefix = "Regex";
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                "UaDetector.Attributes.RegexesAttribute",
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

        string collectionInitializer;
        string regexDefinitions;

        if (property.ElementGenericType == GetGlobalQualifiedName<Browser>())
        {
            var list = DeserializeJson<BrowserRegex>(json);
            regexDefinitions = GenerateBrowserRegexes(list);
            collectionInitializer = GenerateCollectionInitializer(list, property);
        }
        else
        {
            throw new NotSupportedException();
        }

        var fieldName = $"_{property.PropertyName}";

        return $$"""
            {{property.Namespace}}

            partial class {{property.ContainingClass}}
            {
                {{regexDefinitions}}
                
                private static readonly global::System.Collections.Generic.IReadOnlyList<{{property.ElementType}}> {{fieldName}} = {{collectionInitializer}};

                {{property.PropertyAccessibility.ToSyntaxString()}} static partial global::System.Collections.Generic.IReadOnlyList<{{property.ElementType}}> {{property.PropertyName}} =>
                    {{fieldName}}; 
            }
            """;
    }

    private static string GetGlobalQualifiedName<T>()
    {
        return $"global::{typeof(T).FullName}";
    }

    private static EquatableReadOnlyList<T> DeserializeJson<T>(string json)
    {
        try
        {
            var list = JsonSerializer
                .Deserialize<List<T>>(json, SerializerOptions)
                ?.ToEquatableReadOnlyList();

            return list ?? [];
        }
        catch
        {
            // TODO: signal what went wrong
            return [];
        }
    }

    private static string GenerateBrowserRegexes(EquatableReadOnlyList<BrowserRegex> list)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendLine(
                RegexHelper.BuildRegexFieldDeclaration($"{RegexMethodPrefix}{i}", list[i].Regex)
            );
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string GenerateCollectionInitializer(
        EquatableReadOnlyList<BrowserRegex> list,
        PropertyDeclarationInfo property
    )
    {
        if (list.Count == 0)
        {
            return "[]";
        }

        var sb = new StringBuilder();
        var engineType = $"global::{typeof(Engine).FullName}";

        sb.AppendLine("[");

        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendIndentedLine(1, $"new {property.ElementType}")
                .AppendIndentedLine(1, "{")
                .AppendIndentedLine(
                    2,
                    $"{nameof(RuleDefinition<Browser>.Regex)} = {RegexMethodPrefix}{i},"
                )
                .AppendIndentedLine(
                    2,
                    $"{nameof(RuleDefinition<Browser>.Result)} = new {property.ElementGenericType}"
                )
                .AppendIndentedLine(2, "{")
                .AppendIndentedLine(3, $"{nameof(Browser.Name)} = \"{list[i].Name}\",");

            if (list[i].Version is not null)
            {
                sb.AppendIndentedLine(3, $"{nameof(Browser.Version)} = \"{list[i].Version}\",");
            }

            if (list[i].Engine is not null)
            {
                sb.AppendIndentedLine(3, $"{nameof(Browser.Engine)} = new {engineType}")
                    .AppendIndentedLine(3, "{");

                if (list[i].Engine?.Default is { } defaultEngine)
                {
                    sb.AppendIndentedLine(
                        4,
                        $"{nameof(Browser.Engine.Default)} = \"{defaultEngine}\","
                    );
                }

                if (list[i].Engine?.Versions is { Count: > 0 } engineVersions)
                {
                    sb.AppendIndentedLine(
                            4,
                            $"{nameof(Browser.Engine.Versions)} = new Dictionary<string, string>"
                        )
                        .AppendIndentedLine(4, "{");

                    foreach (var version in engineVersions)
                    {
                        sb.AppendIndentedLine(5, $"{{ \"{version.Key}\", \"{version.Value}\" }},");
                    }

                    sb.AppendIndentedLine(4, "},");
                }

                sb.AppendIndentedLine(3, "},");
            }

            sb.AppendIndentedLine(2, "},");
            sb.AppendIndentedLine(1, "},");
        }

        sb.AppendLine("]");

        return sb.ToString();
    }

    private sealed class PropertyDeclarationInfo
    {
        public required string PropertyName { get; init; }
        public required string ResourcePath { get; init; }
        public required string ContainingClass { get; init; }
        public required string? Namespace { get; init; }
        public required string ElementType { get; init; }
        public required string ElementGenericType { get; init; }
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
