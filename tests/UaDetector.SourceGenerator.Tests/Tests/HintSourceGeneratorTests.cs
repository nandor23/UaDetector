using Microsoft.CodeAnalysis.Testing;
using UaDetector.SourceGenerator.Tests.Helpers;

namespace UaDetector.SourceGenerator.Tests.Tests;

public class HintSourceGeneratorTests
{
    private const string AttributeCode = """
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
    public async Task InitializesFrozenDictionary_WhenJsonIsValid()
    {
        const string sourceCode = """
            using System.Collections.Frozen;
            using UaDetector.Attributes;

            namespace UaDetector;

            internal static partial class TestHintParser
            {
                [HintSource("Resources/Hints/test_hints.json")]
                internal static partial FrozenDictionary<string, string> Hints { get; }
            }
            """;

        const string jsonContent = """
            {
                "Chrome": "chrome-browser",
                "Firefox": "firefox-browser",
                "Safari": "safari-browser"
            }
            """;

        const string expectedGeneratedCode = """
            namespace UaDetector;

            static partial class TestHintParser
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
                Sources = { sourceCode, AttributeCode },
                AdditionalFiles = { ("Resources/Hints/test_hints.json", jsonContent) },
                GeneratedSources =
                {
                    (typeof(HintSourceGenerator), "TestHintParser.g.cs", expectedGeneratedCode),
                },
            },
        };

        await test.RunAsync();
    }

    [Test]
    public async Task Fails_WhenPropertyIsNotFrozenDictionary()
    {
        const string sourceCode = """
            using System.Collections.Generic;
            using UaDetector.Attributes;

            namespace UaDetector;

            internal static partial class TestHintParser
            {
                [HintSource("Resources/Hints/test_hints.json")]
                internal static partial Dictionary<string, string> Hints { get; }
            }
            """;

        const string jsonContent = """
            {
                "Chrome": "chrome-browser",
                "Firefox": "firefox-browser",
                "Safari": "safari-browser"
            }
            """;

        var test = new IncrementalGeneratorTest<HintSourceGenerator>
        {
            TestState =
            {
                Sources = { sourceCode, AttributeCode },
                AdditionalFiles = { ("Resources/Hints/test_hints.json", jsonContent) },
                ExpectedDiagnostics =
                {
                    DiagnosticResult.CompilerError("UAD002").WithSpan(8, 5, 9, 70),
                    DiagnosticResult
                        .CompilerError("CS9248")
                        .WithSpan(9, 56, 9, 61)
                        .WithArguments("UaDetector.TestHintParser.Hints"),
                },
            },
        };

        await test.RunAsync();
    }
}
