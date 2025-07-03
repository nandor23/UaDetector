using Microsoft.CodeAnalysis.Testing;
using UaDetector.SourceGenerator.Analyzers;

namespace UaDetector.SourceGenerator.Tests.Tests.Analyzers;

public class HintSourceAnalyzerTests
{
    [Test]
    public async Task ReportDiagnostic_WhenPropertyTypeIsInvalid()
    {
        const string sourceCode = """
            using System.Collections.Frozen;
            using UaDetector.Attributes;

            namespace UaDetector;

            internal static partial class TestHintParser
            {
                [HintSource("Resources/Hints/test_hints.json")]
                internal static partial FrozenDictionary<int, string> Hints { get; }
            }
            """;

        const string attributeCode = """
            namespace UaDetector.Attributes;

            using System;

            [AttributeUsage(AttributeTargets.Property)]
            internal sealed class HintSourceAttribute : Attribute
            {
                public string FilePath { get; }

                public HintSourceAttribute(string filePath)
                {
                    FilePath = filePath;
                }
            }
            """;

        var test = new Helpers.AnalyzerTest<HintSourceAnalyzer>
        {
            TestState =
            {
                Sources = { sourceCode, attributeCode },
                ExpectedDiagnostics =
                {
                    DiagnosticResult.CompilerError("UAD002").WithSpan(9, 59, 9, 64),
                    DiagnosticResult
                        .CompilerError("CS9248")
                        .WithSpan(9, 59, 9, 64)
                        .WithArguments("UaDetector.TestHintParser.Hints"),
                },
            },
        };

        await test.RunAsync();
    }
}
