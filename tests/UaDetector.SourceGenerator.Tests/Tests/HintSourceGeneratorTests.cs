using Microsoft.CodeAnalysis.Testing;
using UaDetector.SourceGenerator.Tests.Helpers;

namespace UaDetector.SourceGenerator.Tests.Tests;

public class HintSourceGeneratorTests
{
    private const string SourceCode = """
        using System.Collections.Frozen;
        using UaDetector.Attributes;

        namespace UaDetector;

        internal static partial class HintParser
        {
            [HintSource("Resources/hints.json")]
            internal static partial FrozenDictionary<string, string> Hints { get; }
        }
        """;

    private const string HintSourceAttributeCode = """
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

    [Test]
    public async Task InitializeFrozenDictionary_WhenJsonIsValid()
    {
        const string jsonContent = """
            {
                "Chrome": "chrome-browser",
                "Firefox": "firefox-browser",
                "Safari": "safari-browser"
            }
            """;

        const string expectedGeneratedCode = """
            namespace UaDetector;

            static partial class HintParser
            {
                private static readonly global::System.Collections.Frozen.FrozenDictionary<string, string> _Hints =
                    global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(
                        new global::System.Collections.Generic.Dictionary<string, string>
                        {
                            { "Chrome", "chrome-browser" },
                            { "Firefox", "firefox-browser" },
                            { "Safari", "safari-browser" },
                        }
                    );

                internal static partial global::System.Collections.Frozen.FrozenDictionary<string, string> Hints => _Hints;
            }

            """;

        var test = new IncrementalGeneratorTest<HintSourceGenerator>
        {
            TestState =
            {
                Sources = { SourceCode, HintSourceAttributeCode },
                AdditionalFiles = { ("Resources/hints.json", jsonContent) },
                GeneratedSources =
                {
                    (
                        typeof(HintSourceGenerator),
                        "HintParser.g.cs",
                        expectedGeneratedCode.ReplaceLineEndings()
                    ),
                },
            },
        };

        await test.RunAsync();
    }

    [Test]
    public async Task ReportDiagnostic_WhenJsonIsInvalid()
    {
        const string jsonContent = """
            {
                "Chrome": "chrome-browser",
                "Firefox": "firefox-browser"
            """;

        var test = new IncrementalGeneratorTest<HintSourceGenerator>
        {
            TestState =
            {
                Sources = { SourceCode, HintSourceAttributeCode },
                AdditionalFiles = { ("Resources/hints.json", jsonContent) },
                ExpectedDiagnostics =
                {
                    DiagnosticResult.CompilerError("UAD001"),
                    DiagnosticResult
                        .CompilerError("CS9248")
                        .WithSpan(9, 62, 9, 67)
                        .WithArguments("UaDetector.HintParser.Hints"),
                },
            },
        };

        await test.RunAsync();
    }
}
