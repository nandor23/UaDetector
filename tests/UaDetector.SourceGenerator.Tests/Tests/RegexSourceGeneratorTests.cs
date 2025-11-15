using Microsoft.CodeAnalysis.Testing;
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
                [RegexSource("Resources/regexes.json")]
                internal static partial IReadOnlyList<{{modelTypeName}}> Regexes { get; }

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
                [RegexSource("Resources/regexes.json")]
                internal static partial IReadOnlyList<{{modelTypeName}}> Regexes { get; }
            }
            """;

    private const string RegexSourceAttributeCode = """
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

    private const string CombinedRegexAttributeCode = """
        namespace UaDetector.Attributes;

        using System;

        [AttributeUsage(AttributeTargets.Property)]
        internal sealed class CombinedRegexAttribute : Attribute;
        """;

    [Test]
    [MethodDataSource(nameof(TestData))]
    public async Task GenerateRegexes_WithCombinedRegex_WhenJsonIsInvalid(
        SourceGeneratorTestCase testCase,
        bool isLiteMode
    )
    {
        string expectedGeneratedCode = $$"""
            namespace UaDetector;

            partial class Parser
            {
                private static readonly global::System.Text.RegularExpressions.Regex {{testCase.ModelTypeName}}Regex0 = 
                    new global::System.Text.RegularExpressions.Regex(
                        @"{{RegexBuilder.BuildPattern(testCase.RegexPattern)}}", 
                        {{GetRegexOptions(isLiteMode)}});

                private static readonly global::System.Collections.Generic.IReadOnlyList<global::UaDetector.Models.{{testCase.ModelTypeName}}> _Regexes = {{IndentListContent(
                testCase.DeserializedModels
            )}};

                internal static partial global::System.Collections.Generic.IReadOnlyList<global::UaDetector.Models.{{testCase.ModelTypeName}}> Regexes => _Regexes;

                private static readonly global::System.Text.RegularExpressions.Regex _CombinedRegex = 
                    new global::System.Text.RegularExpressions.Regex(
                        @"{{RegexBuilder.BuildPattern(testCase.RegexPattern)}}", 
                        {{GetRegexOptions(isLiteMode)}});

                private static partial global::System.Text.RegularExpressions.Regex CombinedRegex => _CombinedRegex;
            }

            """;

        var test = new IncrementalGeneratorTest<RegexSourceGenerator>
        {
            IsLiteMode = isLiteMode,
            TestState =
            {
                Sources =
                {
                    GetSourceCodeWithCombinedRegex(testCase.ModelTypeName),
                    RegexSourceAttributeCode,
                    CombinedRegexAttributeCode,
                },
                AdditionalFiles = { ("Resources/regexes.json", testCase.JsonContent) },
                GeneratedSources =
                {
                    (typeof(RegexSourceGenerator), "Parser.g.cs", expectedGeneratedCode),
                },
            },
        };

        foreach (var modelSourceCode in testCase.ModelSourceCodes)
        {
            test.TestState.Sources.Add(modelSourceCode);
        }

        await test.RunAsync();
    }

    [Test]
    [MethodDataSource(nameof(TestData))]
    public async Task GenerateRegexes_WithoutCombinedRegex_WhenJsonIsInvalid(
        SourceGeneratorTestCase testCase,
        bool isLiteMode
    )
    {
        string expectedGeneratedCode = $$"""
            namespace UaDetector;

            partial class Parser
            {
                private static readonly global::System.Text.RegularExpressions.Regex {{testCase.ModelTypeName}}Regex0 = 
                    new global::System.Text.RegularExpressions.Regex(
                        @"{{RegexBuilder.BuildPattern(testCase.RegexPattern)}}", 
                        {{GetRegexOptions(isLiteMode)}});

                private static readonly global::System.Collections.Generic.IReadOnlyList<global::UaDetector.Models.{{testCase.ModelTypeName}}> _Regexes = {{IndentListContent(
                testCase.DeserializedModels
            )}};

                internal static partial global::System.Collections.Generic.IReadOnlyList<global::UaDetector.Models.{{testCase.ModelTypeName}}> Regexes => _Regexes;
            }

            """;

        var test = new IncrementalGeneratorTest<RegexSourceGenerator>
        {
            IsLiteMode = isLiteMode,
            TestState =
            {
                Sources =
                {
                    GetSourceCodeWithoutCombinedRegex(testCase.ModelTypeName),
                    RegexSourceAttributeCode,
                },
                AdditionalFiles = { ("Resources/regexes.json", testCase.JsonContent) },
                GeneratedSources =
                {
                    (typeof(RegexSourceGenerator), "Parser.g.cs", expectedGeneratedCode),
                },
            },
        };

        foreach (var modelSourceCode in testCase.ModelSourceCodes)
        {
            test.TestState.Sources.Add(modelSourceCode);
        }

        await test.RunAsync();
    }

    [Test]
    [MethodDataSource(nameof(TestData))]
    public async Task ReportDiagnostic_WhenJsonIsInvalid(
        SourceGeneratorTestCase testCase,
        bool isLiteMode
    )
    {
        var test = new IncrementalGeneratorTest<RegexSourceGenerator>
        {
            IsLiteMode = isLiteMode,
            TestState =
            {
                Sources =
                {
                    GetSourceCodeWithCombinedRegex(testCase.ModelTypeName),
                    RegexSourceAttributeCode,
                    CombinedRegexAttributeCode,
                },
                AdditionalFiles = { ("Resources/regexes.json", testCase.JsonContent[..^1]) },
                ExpectedDiagnostics = { DiagnosticResult.CompilerError("UAD001") },
            },
        };

        foreach (var modelSourceCode in testCase.ModelSourceCodes)
        {
            test.TestState.Sources.Add(modelSourceCode);
        }

        foreach (var diagnostic in testCase.ExpectedDiagnostics)
        {
            test.TestState.ExpectedDiagnostics.Add(diagnostic);
        }

        await test.RunAsync();
    }

    public static IEnumerable<Func<(SourceGeneratorTestCase, bool)>> TestData()
    {
        IEnumerable<SourceGeneratorTestCase> baseTestCases =
        [
            new()
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
                DeserializedModels = """
                    [
                        new global::UaDetector.Models.Client
                        {
                            Regex = ClientRegex0,
                            Name = "iTunes",
                            Version = "$1",
                        },
                    ]
                    """,
                ModelSourceCodes =
                [
                    """
                        using System.Text.RegularExpressions;

                        namespace UaDetector.Models;

                        public sealed class Client
                        {
                            public required Regex Regex { get; init; }
                            public required string Name { get; init; }
                            public required string Version { get; init; }
                        }
                        """,
                ],
                ExpectedDiagnostics =
                [
                    DiagnosticResult
                        .CompilerError("CS9248")
                        .WithSpan(11, 51, 11, 58)
                        .WithArguments("UaDetector.Parser.Regexes"),
                    DiagnosticResult
                        .CompilerError("CS9248")
                        .WithSpan(14, 34, 14, 47)
                        .WithArguments("UaDetector.Parser.CombinedRegex"),
                ],
            },
            new()
            {
                ModelTypeName = "Browser",
                RegexPattern = "Brave",
                JsonContent = """
                    [
                        {
                            "regex": "Brave",
                            "name": "Brave",
                            "version": "$1",
                            "engine": {
                                "default": "Blink",
                                "versions": {
                                    "28": "Blink"
                                }
                            }
                        }
                    ]
                    """,
                DeserializedModels = """
                    [
                        new global::UaDetector.Models.Browser
                        {
                            Regex = BrowserRegex0,
                            Name = "Brave",
                            Version = "$1",
                            Engine = new global::UaDetector.Models.BrowserEngine
                            {
                                Default = "Blink",
                                Versions = new global::System.Collections.Generic.Dictionary<string, string>
                                {
                                    { "28", "Blink" },
                                },
                            },
                        },
                    ]
                    """,
                ModelSourceCodes =
                [
                    """
                        using System.Text.RegularExpressions;

                        namespace UaDetector.Models;

                        internal sealed class Browser
                        {
                            public required Regex Regex { get; init; }
                            public required string Name { get; init; }
                            public string? Version { get; init; }
                            public BrowserEngine? Engine { get; init; }
                        }
                        """,
                    """
                        namespace UaDetector.Models;

                        using System.Collections.Generic;

                        internal sealed class BrowserEngine
                        {
                            public string? Default { get; init; }
                            public IReadOnlyDictionary<string, string>? Versions { get; init; }
                        }
                        """,
                ],
                ExpectedDiagnostics =
                [
                    DiagnosticResult
                        .CompilerError("CS9248")
                        .WithSpan(11, 52, 11, 59)
                        .WithArguments("UaDetector.Parser.Regexes"),
                    DiagnosticResult
                        .CompilerError("CS9248")
                        .WithSpan(14, 34, 14, 47)
                        .WithArguments("UaDetector.Parser.CombinedRegex"),
                ],
            },
            new()
            {
                ModelTypeName = "Engine",
                RegexPattern = "Maple",
                JsonContent = """
                    [
                        {
                            "regex": "Maple",
                            "name": "Maple"
                        }
                    ]
                    """,
                DeserializedModels = """
                    [
                        new global::UaDetector.Models.Engine
                        {
                            Regex = EngineRegex0,
                            Name = "Maple",
                        },
                    ]
                    """,
                ModelSourceCodes =
                [
                    """
                        using System.Text.RegularExpressions;

                        namespace UaDetector.Models;

                        internal sealed class Engine
                        {
                            public required Regex Regex { get; init; }
                            public required string Name { get; init; }
                        }
                        """,
                ],
                ExpectedDiagnostics =
                [
                    DiagnosticResult
                        .CompilerError("CS9248")
                        .WithSpan(11, 51, 11, 58)
                        .WithArguments("UaDetector.Parser.Regexes"),
                    DiagnosticResult
                        .CompilerError("CS9248")
                        .WithSpan(14, 34, 14, 47)
                        .WithArguments("UaDetector.Parser.CombinedRegex"),
                ],
            },
            new()
            {
                ModelTypeName = "Os",
                RegexPattern = "Web0S",
                JsonContent = """
                    [
                        {
                            "regex": "Web0S",
                            "name": "webOS",
                            "version": "$1"
                        }
                    ]
                    """,
                DeserializedModels = """
                    [
                        new global::UaDetector.Models.Os
                        {
                            Regex = OsRegex0,
                            Name = "webOS",
                            Version = "$1",
                        },
                    ]
                    """,
                ModelSourceCodes =
                [
                    """
                        using System.Text.RegularExpressions;
                        using System.Collections.Generic;

                        namespace UaDetector.Models;

                        internal sealed class Os
                        {
                            public required Regex Regex { get; init; }
                            public required string Name { get; init; }
                            public string? Version { get; init; }
                            public IReadOnlyList<OsVersion>? Versions { get; init; }
                        }
                        """,
                    """
                        using System.Text.RegularExpressions;

                        namespace UaDetector.Models;

                        internal sealed class OsVersion
                        {
                            public required Regex Regex { get; init; }
                            public required string Version { get; init; }
                        }
                        """,
                ],
                ExpectedDiagnostics =
                [
                    DiagnosticResult
                        .CompilerError("CS9248")
                        .WithSpan(11, 47, 11, 54)
                        .WithArguments("UaDetector.Parser.Regexes"),
                    DiagnosticResult
                        .CompilerError("CS9248")
                        .WithSpan(14, 34, 14, 47)
                        .WithArguments("UaDetector.Parser.CombinedRegex"),
                ],
            },
            new()
            {
                ModelTypeName = "Device",
                RegexPattern = "Dell",
                JsonContent = """
                    [
                        {
                            "brand": "Dell",
                            "regex": "Dell",
                            "type": 0
                        }
                    ]
                    """,
                DeserializedModels = """
                    [
                        new global::UaDetector.Models.Device
                        {
                            Regex = DeviceRegex0,
                            Brand = "Dell",
                            Type = (global::UaDetector.Abstractions.Enums.DeviceType)0,
                        },
                    ]
                    """,
                ModelSourceCodes =
                [
                    """
                        using System.Text.RegularExpressions;
                        using UaDetector.Abstractions.Enums;
                        using System.Collections.Generic;

                        namespace UaDetector.Models;

                        internal sealed class Device
                        {
                            public required Regex Regex { get; init; }
                            public required string Brand { get; init; }
                            public DeviceType? Type { get; init; }
                            public string? Model { get; init; }
                            public IReadOnlyList<DeviceModel>? ModelVariants { get; init; }
                        }
                        """,
                    """
                        using System.Text.RegularExpressions;
                        using UaDetector.Abstractions.Enums;

                        namespace UaDetector.Models;

                        internal sealed class DeviceModel
                        {
                            public required Regex Regex { get; init; }
                            public DeviceType? Type { get; init; }
                            public string? Brand { get; init; }
                            public string? Name { get; init; }
                        }
                        """,
                    """
                        namespace UaDetector.Abstractions.Enums;

                        public enum DeviceType;
                        """,
                ],
                ExpectedDiagnostics =
                [
                    DiagnosticResult
                        .CompilerError("CS9248")
                        .WithSpan(11, 51, 11, 58)
                        .WithArguments("UaDetector.Parser.Regexes"),
                    DiagnosticResult
                        .CompilerError("CS9248")
                        .WithSpan(14, 34, 14, 47)
                        .WithArguments("UaDetector.Parser.CombinedRegex"),
                ],
            },
            new()
            {
                ModelTypeName = "Bot",
                RegexPattern = "Amazonbot",
                JsonContent = """
                    [
                        {
                            "regex": "Amazonbot",
                            "name": "Amazon Bot",
                            "category": 3,
                            "url": "https://developer.amazon.com/support/amazonbot",
                            "producer": {
                                "name": "Amazon.com, Inc.",
                                "url": "https://www.amazon.com/"
                            }
                        }
                    ]
                    """,
                DeserializedModels = """
                    [
                        new global::UaDetector.Models.Bot
                        {
                            Regex = BotRegex0,
                            Name = "Amazon Bot",
                            Category = (global::UaDetector.Abstractions.Enums.BotCategory)3,
                            Url = "https://developer.amazon.com/support/amazonbot",
                            Producer = new global::UaDetector.Models.BotProducer
                            {
                                Name = "Amazon.com, Inc.",
                                Url = "https://www.amazon.com/",
                            },
                        },
                    ]
                    """,
                ModelSourceCodes =
                [
                    """
                        using System.Text.RegularExpressions;
                        using UaDetector.Abstractions.Enums;

                        namespace UaDetector.Models;

                        internal sealed class Bot
                        {
                            public required Regex Regex { get; init; }
                            public required string Name { get; init; }
                            public BotCategory? Category { get; init; }
                            public string? Url { get; init; }
                            public BotProducer? Producer { get; init; }
                        }
                        """,
                    """
                        namespace UaDetector.Models;

                        internal sealed class BotProducer
                        {
                            public string? Name { get; init; }
                            public string? Url { get; init; }
                        }
                        """,
                    """
                        namespace UaDetector.Abstractions.Enums;

                        public enum BotCategory;
                        """,
                ],
                ExpectedDiagnostics =
                [
                    DiagnosticResult
                        .CompilerError("CS9248")
                        .WithSpan(11, 48, 11, 55)
                        .WithArguments("UaDetector.Parser.Regexes"),
                    DiagnosticResult
                        .CompilerError("CS9248")
                        .WithSpan(14, 34, 14, 47)
                        .WithArguments("UaDetector.Parser.CombinedRegex"),
                ],
            },
            new()
            {
                ModelTypeName = "VendorFragment",
                RegexPattern = "Dell",
                JsonContent = """
                    [
                        {
                            "brand": "Dell",
                            "regexes": [
                                "Dell"
                            ]
                        }
                    ]
                    """,
                DeserializedModels = """
                    [
                        new global::UaDetector.Models.VendorFragment
                        {
                            Brand = "Dell",
                            Regexes = new global::System.Text.RegularExpressions.Regex[]
                            {
                                VendorFragmentRegex0,
                            },
                        },
                    ]
                    """,
                ModelSourceCodes =
                [
                    """
                        using System.Text.RegularExpressions;
                        using System.Collections.Generic;

                        namespace UaDetector.Models;

                        internal sealed class VendorFragment
                        {
                            public required string Brand { get; init; }
                            public required IReadOnlyList<Regex> Regexes { get; init; }
                        }
                        """,
                ],
                ExpectedDiagnostics =
                [
                    DiagnosticResult
                        .CompilerError("CS9248")
                        .WithSpan(11, 59, 11, 66)
                        .WithArguments("UaDetector.Parser.Regexes"),
                    DiagnosticResult
                        .CompilerError("CS9248")
                        .WithSpan(14, 34, 14, 47)
                        .WithArguments("UaDetector.Parser.CombinedRegex"),
                ],
            },
        ];

        foreach (var testCase in baseTestCases)
        {
            yield return () => (testCase, false);
            yield return () => (testCase, true);
        }
    }

    private static string IndentListContent(string input)
    {
        var indent = new string(' ', 4);

        var lines = input.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            lines[i] = indent + lines[i];
        }
        return string.Join('\n', lines);
    }

    private static string GetRegexOptions(bool isLiteMode)
    {
        const string ignoreCase = "global::System.Text.RegularExpressions.RegexOptions.IgnoreCase";
        const string compiled = "global::System.Text.RegularExpressions.RegexOptions.Compiled";

        return isLiteMode ? ignoreCase : $"{ignoreCase} |\n            {compiled}";
    }

    public class SourceGeneratorTestCase
    {
        public required string ModelTypeName { get; init; }
        public required string RegexPattern { get; init; }
        public required string JsonContent { get; init; }
        public required string DeserializedModels { get; init; }
        public required IReadOnlyList<string> ModelSourceCodes { get; init; }
        public required IReadOnlyList<DiagnosticResult> ExpectedDiagnostics { get; init; }
    }
}
