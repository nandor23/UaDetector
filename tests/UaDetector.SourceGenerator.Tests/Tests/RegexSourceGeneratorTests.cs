using UaDetector.SourceGenerator.Tests.Helpers;
using UaDetector.SourceGenerator.Utilities;

namespace UaDetector.SourceGenerator.Tests.Tests;

public class RegexSourceGeneratorTests
{
    static string GetSourceCodeWithCombinedRegex(string modelTypeName) =>
        $$"""
            using System.Text.RegularExpressions;
            using System.Collections.Generic;
            using UaDetector.Attributes;
            using UaDetector.Models;

            namespace UaDetector;

            internal sealed partial class Parser
            {
                [RegexSource("Resources/clients.json")]
                internal static partial IReadOnlyList<{{modelTypeName}}> Clients { get; }

                [CombinedRegex]
                private static partial Regex CombinedRegex { get; }
            }
            """;

    static string GetSourceCodeWithoutCombinedRegex(string modelTypeName) =>
        $$"""
            using System.Text.RegularExpressions;
            using System.Collections.Generic;
            using UaDetector.Attributes;
            using UaDetector.Models;

            namespace UaDetector;

            internal sealed partial class Parser
            {
                [RegexSource("Resources/clients.json")]
                internal static partial IReadOnlyList<{{modelTypeName}}> Clients { get; }
            }
            """;

    private const string RegexSourceAttribute = """
        namespace UaDetector.Attributes;

        using System;

        [AttributeUsage(AttributeTargets.Property)]
        internal sealed class RegexSourceAttribute : Attribute
        {
            public string FilePath { get; }
            public string? RegexSuffix { get; }

            public RegexSourceAttribute(string filePath, string? regexSuffix = null)
            {
                FilePath = filePath;
                RegexSuffix = regexSuffix;
            }
        }
        """;

    private const string CombinedRegexAttribute = """
        namespace UaDetector.Attributes;

        using System;

        [AttributeUsage(AttributeTargets.Property)]
        internal sealed class CombinedRegexAttribute : Attribute;
        """;

    [Test]
    [MethodDataSource(nameof(TestData))]
    public async Task GenerateRegexes_WithCombinedRegex_WhenJsonIsInvalid(
        SourceGeneratorTestCase testCase
    )
    {
        string expectedGeneratedCode = $$"""
            namespace UaDetector;

            partial class Parser
            {
                private static readonly global::System.Text.RegularExpressions.Regex ClientRegex0 = 
                    new global::System.Text.RegularExpressions.Regex(
                        @"{{RegexBuilder.BuildPattern(testCase.RegexPattern)}}", 
                        global::System.Text.RegularExpressions.RegexOptions.IgnoreCase | 
                        global::System.Text.RegularExpressions.RegexOptions.Compiled);

                private static readonly global::System.Collections.Generic.IReadOnlyList<global::UaDetector.Models.{{testCase.ModelTypeName}}> _Clients = [
                    {{IndentListContent(testCase.ListContent)}}
                ];

                internal static partial global::System.Collections.Generic.IReadOnlyList<global::UaDetector.Models.{{testCase.ModelTypeName}}> Clients => _Clients;

                private static readonly global::System.Text.RegularExpressions.Regex _CombinedRegex = 
                    new global::System.Text.RegularExpressions.Regex(
                        @"{{RegexBuilder.BuildPattern(testCase.RegexPattern)}}", 
                        global::System.Text.RegularExpressions.RegexOptions.IgnoreCase | 
                        global::System.Text.RegularExpressions.RegexOptions.Compiled);

                private static partial global::System.Text.RegularExpressions.Regex CombinedRegex => _CombinedRegex;
            }

            """;

        var test = new IncrementalGeneratorTest<RegexSourceGenerator>
        {
            TestState =
            {
                Sources =
                {
                    GetSourceCodeWithCombinedRegex(testCase.ModelTypeName),
                    RegexSourceAttribute,
                    CombinedRegexAttribute,
                    testCase.ModelSourceCode,
                },
                AdditionalFiles = { ("Resources/clients.json", testCase.JsonContent) },
                GeneratedSources =
                {
                    (typeof(RegexSourceGenerator), "Parser.g.cs", expectedGeneratedCode),
                },
            },
        };

        await test.RunAsync();
    }

    [Test]
    [MethodDataSource(nameof(TestData))]
    public async Task GenerateRegexes_WithoutCombinedRegex_WhenJsonIsInvalid(
        SourceGeneratorTestCase testCase
    )
    {
        string expectedGeneratedCode = $$"""
            namespace UaDetector;

            partial class Parser
            {
                private static readonly global::System.Text.RegularExpressions.Regex ClientRegex0 = 
                    new global::System.Text.RegularExpressions.Regex(
                        @"{{RegexBuilder.BuildPattern(testCase.RegexPattern)}}", 
                        global::System.Text.RegularExpressions.RegexOptions.IgnoreCase | 
                        global::System.Text.RegularExpressions.RegexOptions.Compiled);

                private static readonly global::System.Collections.Generic.IReadOnlyList<global::UaDetector.Models.{{testCase.ModelTypeName}}> _Clients = [
                    {{IndentListContent(testCase.ListContent)}}
                ];

                internal static partial global::System.Collections.Generic.IReadOnlyList<global::UaDetector.Models.{{testCase.ModelTypeName}}> Clients => _Clients;
            }

            """;

        var test = new IncrementalGeneratorTest<RegexSourceGenerator>
        {
            TestState =
            {
                Sources =
                {
                    GetSourceCodeWithoutCombinedRegex(testCase.ModelTypeName),
                    RegexSourceAttribute,
                    testCase.ModelSourceCode,
                },
                AdditionalFiles = { ("Resources/clients.json", testCase.JsonContent) },
                GeneratedSources =
                {
                    (typeof(RegexSourceGenerator), "Parser.g.cs", expectedGeneratedCode),
                },
            },
        };

        await test.RunAsync();
    }

    public static IEnumerable<Func<SourceGeneratorTestCase>> TestData()
    {
        yield return () =>
            new SourceGeneratorTestCase
            {
                ModelTypeName = "Client",
                RegexPattern = "iTunes",
                JsonContent = """
                    [
                        {
                          "regex": "iTunes",
                          "name": "iTunes", 
                          "version": "$1"
                        }
                    ]
                    """,
                ListContent = """
                    new global::UaDetector.Models.Client
                    {
                        Regex = ClientRegex0,
                        Name = "iTunes",
                        Version = "$1",
                    },
                    """,
                ModelSourceCode = """
                    using System.Text.RegularExpressions;

                    namespace UaDetector.Models;

                    public sealed class Client
                    {
                        public required Regex Regex { get; init; }
                        public required string Name { get; init; }
                        public required string Version { get; init; }
                    }
                    """,
            };
    }

    private static string IndentListContent(string input)
    {
        var indent = new string(' ', 8);

        var lines = input.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            lines[i] = indent + lines[i];
        }
        return string.Join('\n', lines);
    }

    public class SourceGeneratorTestCase
    {
        public required string ModelTypeName { get; init; }
        public required string RegexPattern { get; init; }
        public required string JsonContent { get; init; }
        public required string ListContent { get; init; }
        public required string ModelSourceCode { get; init; }
    }
}
