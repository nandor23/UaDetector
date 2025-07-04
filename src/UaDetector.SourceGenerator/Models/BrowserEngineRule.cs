using UaDetector.SourceGenerator.Collections;

namespace UaDetector.SourceGenerator.Models;

public sealed record BrowserEngineRule
{
    public string? Default { get; init; }
    public EquatableReadOnlyDictionary<string, string>? Versions { get; init; }
}
