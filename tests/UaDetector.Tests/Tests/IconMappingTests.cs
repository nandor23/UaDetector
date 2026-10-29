using System.Buffers.Binary;
using System.Collections.Immutable;
using System.Text.Json;
using Shouldly;
using UaDetector.Registries;

namespace UaDetector.Tests.Tests;

public class IconMappingTests
{
    private const int IconSize = 48;

    private static readonly string IconsPath = Path.Combine(AppContext.BaseDirectory, "Icons");

    private static readonly Dictionary<string, Dictionary<string, string[]>> Mapping =
        JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string[]>>>(
            File.ReadAllText(Path.Combine(IconsPath, "icons.json"))
        )!;

    [Test]
    [Arguments("browsers")]
    [Arguments("os")]
    public void Icons_ShouldOnlyContainKnownNames(string category)
    {
        GetNames(category).Except(GetKnownNames(category)).ShouldBeEmpty();
    }

    [Test]
    [Arguments("browsers")]
    [Arguments("os")]
    public void Icons_ShouldNotContainDuplicateNames(string category)
    {
        GetNames(category)
            .GroupBy(name => name)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ShouldBeEmpty();
    }

    [Test]
    [Arguments("browsers")]
    [Arguments("os")]
    public void Icons_ShouldReferenceExistingFiles(string category)
    {
        Mapping[category]
            .Keys.Where(fileName => !File.Exists(Path.Combine(IconsPath, category, fileName)))
            .ShouldBeEmpty();
    }

    [Test]
    [Arguments("browsers")]
    [Arguments("os")]
    public void IconFiles_ShouldBeReferenced(string category)
    {
        Directory
            .EnumerateFiles(Path.Combine(IconsPath, category), "*.png")
            .Select(Path.GetFileName)
            .Except(Mapping[category].Keys)
            .ShouldBeEmpty();
    }

    [Test]
    public void IconFiles_ShouldHaveExpectedSize()
    {
        Directory
            .EnumerateFiles(IconsPath, "*.png", SearchOption.AllDirectories)
            .Where(path => ReadPngSize(path) != (IconSize, IconSize))
            .Select(path => Path.GetRelativePath(IconsPath, path))
            .ShouldBeEmpty();
    }

    [Test]
    [Arguments("browsers")]
    [Arguments("os")]
    public void PrintNamesWithoutIcons(string category)
    {
        var namesWithoutIcons = GetKnownNames(category)
            .Except(GetNames(category))
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToList();

        Console.WriteLine($"{namesWithoutIcons.Count} {category} without an icon:");

        foreach (var name in namesWithoutIcons)
        {
            Console.WriteLine(name);
        }
    }

    private static IEnumerable<string> GetNames(string category)
    {
        return Mapping[category].Values.SelectMany(names => names);
    }

    private static ImmutableArray<string> GetKnownNames(string category)
    {
        return category switch
        {
            "browsers" => BrowserRegistry.BrowserCodeMappings.Values,
            "os" => OsRegistry.OsCodeMappings.Values,
            _ => throw new ArgumentOutOfRangeException(nameof(category)),
        };
    }

    private static (int Width, int Height) ReadPngSize(string path)
    {
        Span<byte> header = stackalloc byte[24];
        using var stream = File.OpenRead(path);
        stream.ReadExactly(header);

        return (
            BinaryPrimitives.ReadInt32BigEndian(header[16..]),
            BinaryPrimitives.ReadInt32BigEndian(header[20..])
        );
    }
}
