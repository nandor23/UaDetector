namespace UaDetector.YamlJsonConverter.Fixtures;

public sealed class OsFixtureYaml
{
    public required string UserAgent { get; init; }
    public required Os Os { get; init; }
}

public sealed class Os
{
    public required string Name { get; init; }
    public string? Version { get; init; }
    public string? Platform { get; init; }
    public string? Family { get; init; }
}
