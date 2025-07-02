using UaDetector.SourceGenerator.Tests.Helpers;

namespace UaDetector.SourceGenerator.Tests.Tests;

public class HintSourceGeneratorTests
{
    [Test]
    public async Task HintSourceGenerator_GeneratesCorrectCode_WhenValidJsonProvided()
    {
        const string sourceCode = """
            using System.Collections.Frozen;
            using UaDetector.Attributes;

            namespace UaDetector.Tests;

            internal static partial class TestHintParser
            {
                [HintSource("Resources/Hints/test_hints.json")]
                internal static partial FrozenDictionary<string, string> Hints { get; }
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

        const string jsonContent = """
            {
                "Chrome": "chrome-browser",
                "Firefox": "firefox-browser",
                "Safari": "safari-browser"
            }
            """;

        const string expectedGeneratedCode = """
            namespace UaDetector.Tests;

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
                Sources = { sourceCode, attributeCode },
                AdditionalFiles = { ("Resources/Hints/test_hints.json", jsonContent) },
                GeneratedSources =
                {
                    (typeof(HintSourceGenerator), "TestHintParser.g.cs", expectedGeneratedCode),
                },
            },
        };

        await test.RunAsync();
    }
}
