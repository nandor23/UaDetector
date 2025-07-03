using Microsoft.CodeAnalysis.Testing;
using UaDetector.SourceGenerator.Analyzers;

namespace UaDetector.SourceGenerator.Tests.Tests.Analyzers;

public class RegexSourceAnalyzerTests
{
    private const string AttributeCode = """
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

    [Test]
    public async Task ReportDiagnostic_WhenModelTypeIsInvalid()
    {
        const string sourceCode = """
            using System.Text.RegularExpressions;
            using System.Collections.Generic;
            using UaDetector.Attributes;

            namespace UaDetector;

            internal sealed partial class Parser
            {
                [RegexSource("Resources/regexes.json")]
                internal static partial IReadOnlyList<string> Regexes { get; }
            }
            """;

        var test = new Helpers.AnalyzerTest<RegexSourceAnalyzer>
        {
            TestState =
            {
                Sources = { sourceCode, AttributeCode },
                ExpectedDiagnostics =
                {
                    DiagnosticResult
                        .CompilerError("CS9248")
                        .WithSpan(10, 51, 10, 58)
                        .WithArguments("UaDetector.Parser.Regexes"),
                    DiagnosticResult.CompilerError("UAD003").WithSpan(10, 51, 10, 58),
                },
            },
        };

        await test.RunAsync();
    }

    [Test]
    public async Task ReportDiagnostic_WhenCollectionTypeIsInvalid()
    {
        const string sourceCode = """
            using System.Text.RegularExpressions;
            using System.Collections.Generic;
            using UaDetector.Attributes;
            using UaDetector.Models;

            namespace UaDetector;

            internal sealed partial class Parser
            {
                [RegexSource("Resources/regexes.json")]
                internal static partial IEnumerable<Engine> Regexes { get; }
            }
            """;

        const string modelSourceCode = """
            using System.Text.RegularExpressions;

            namespace UaDetector.Models;

            internal sealed class Engine
            {
                public required Regex Regex { get; init; }
                public required string Name { get; init; }
            }
            """;

        var test = new Helpers.AnalyzerTest<RegexSourceAnalyzer>
        {
            TestState =
            {
                Sources = { sourceCode, modelSourceCode, AttributeCode },
                ExpectedDiagnostics =
                {
                    DiagnosticResult
                        .CompilerError("CS9248")
                        .WithSpan(11, 49, 11, 56)
                        .WithArguments("UaDetector.Parser.Regexes"),
                    DiagnosticResult.CompilerError("UAD003").WithSpan(11, 49, 11, 56),
                },
            },
        };

        await test.RunAsync();
    }
}
