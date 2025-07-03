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
                private static readonly global::System.Text.RegularExpressions.Regex {{testCase.ModelTypeName}}Regex0 = 
                    new global::System.Text.RegularExpressions.Regex(
                        @"{{RegexBuilder.BuildPattern(testCase.RegexPattern)}}", 
                        global::System.Text.RegularExpressions.RegexOptions.IgnoreCase | 
                        global::System.Text.RegularExpressions.RegexOptions.Compiled);

                private static readonly global::System.Collections.Generic.IReadOnlyList<global::UaDetector.Models.{{testCase.ModelTypeName}}> _Regexes = {{IndentListContent(
                testCase.DeserializedModels
            )}};

                internal static partial global::System.Collections.Generic.IReadOnlyList<global::UaDetector.Models.{{testCase.ModelTypeName}}> Regexes => _Regexes;

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
        SourceGeneratorTestCase testCase
    )
    {
        string expectedGeneratedCode = $$"""
            namespace UaDetector;

            partial class Parser
            {
                private static readonly global::System.Text.RegularExpressions.Regex {{testCase.ModelTypeName}}Regex0 = 
                    new global::System.Text.RegularExpressions.Regex(
                        @"{{RegexBuilder.BuildPattern(testCase.RegexPattern)}}", 
                        global::System.Text.RegularExpressions.RegexOptions.IgnoreCase | 
                        global::System.Text.RegularExpressions.RegexOptions.Compiled);

                private static readonly global::System.Collections.Generic.IReadOnlyList<global::UaDetector.Models.{{testCase.ModelTypeName}}> _Regexes = {{IndentListContent(
                testCase.DeserializedModels
            )}};

                internal static partial global::System.Collections.Generic.IReadOnlyList<global::UaDetector.Models.{{testCase.ModelTypeName}}> Regexes => _Regexes;
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
            };

        yield return () =>
            new SourceGeneratorTestCase
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
            };

        yield return () =>
            new SourceGeneratorTestCase
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
            };

        yield return () =>
            new SourceGeneratorTestCase
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
            };

        yield return () =>
            new SourceGeneratorTestCase
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
            };
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

    public class SourceGeneratorTestCase
    {
        public required string ModelTypeName { get; init; }
        public required string RegexPattern { get; init; }
        public required string JsonContent { get; init; }
        public required string DeserializedModels { get; init; }
        public required IReadOnlyList<string> ModelSourceCodes { get; init; }
    }
}
