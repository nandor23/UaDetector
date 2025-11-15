using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace UaDetector.SourceGenerator.Tests.Helpers;

public class IncrementalGeneratorTest<TSourceGenerator>
    : CSharpSourceGeneratorTest<TSourceGenerator, DefaultVerifier>
    where TSourceGenerator : IIncrementalGenerator, new()
{
    public bool IsLiteMode { get; set; } = false;

    private static LanguageVersion LanguageVersion => LanguageVersion.CSharp13;

    public IncrementalGeneratorTest()
    {
        TestState.ReferenceAssemblies = ReferenceAssemblies.Net.Net90;
    }

    protected override CompilationOptions CreateCompilationOptions()
    {
        var compilationOptions = (CSharpCompilationOptions)base.CreateCompilationOptions();

        return compilationOptions
            .WithSpecificDiagnosticOptions(
                compilationOptions.SpecificDiagnosticOptions.SetItems(
                    GetNullableWarningsFromCompiler()
                )
            )
            .WithNullableContextOptions(NullableContextOptions.Enable);
    }

    private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
    {
        string[] args = ["/warnaserror:nullable"];
        var commandLineArguments = CSharpCommandLineParser.Default.Parse(
            args,
            baseDirectory: Environment.CurrentDirectory,
            sdkDirectory: Environment.CurrentDirectory
        );
        return commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;
    }

    protected override ParseOptions CreateParseOptions()
    {
        var parseOptions = ((CSharpParseOptions)base.CreateParseOptions()).WithLanguageVersion(
            LanguageVersion
        );

        if (IsLiteMode)
        {
            parseOptions = parseOptions.WithPreprocessorSymbols("UADETECTOR_LITE");
        }

        return parseOptions;
    }
}
