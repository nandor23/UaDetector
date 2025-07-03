using Microsoft.CodeAnalysis.Testing;
using UaDetector.SourceGenerator.Analyzers;

namespace UaDetector.SourceGenerator.Tests.Tests.Analyzers;

public class CombinedRegexAnalyzerTests
{
    [Test]
    public async Task ReportDiagnostic_WhenRegexSourceAttributeIsMissing()
    {
        const string sourceCode = """
            using System.Collections.Frozen;
            using UaDetector.Attributes;
            using System.Text.RegularExpressions;

            namespace UaDetector;

            internal static partial class Parser
            {
                [CombinedRegex]
                private static partial Regex CombinedRegex { get; }
            }
            """;

        const string attributeCode = """
            namespace UaDetector.Attributes;

            using System;

            [AttributeUsage(AttributeTargets.Property)]
            internal sealed class CombinedRegexAttribute : Attribute;
            """;

        var test = new Helpers.AnalyzerTest<CombinedRegexAnalyzer>
        {
            TestState =
            {
                Sources = { sourceCode, attributeCode },
                ExpectedDiagnostics =
                {
                    DiagnosticResult
                        .CompilerError("CS9248")
                        .WithSpan(10, 34, 10, 47)
                        .WithArguments("UaDetector.Parser.CombinedRegex"),
                    DiagnosticResult.CompilerError("UAD004").WithSpan(10, 34, 10, 47),
                },
            },
        };

        await test.RunAsync();
    }
}
