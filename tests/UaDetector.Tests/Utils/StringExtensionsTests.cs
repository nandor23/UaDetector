using Shouldly;

using UaDetector.Utils;

namespace UaDetector.Tests.Utils;

public class StringExtensionsTests
{
    [Test]
    [MethodDataSource(nameof(CollapseSpacesTestData))]
    public void CollapseSpaces_WithExtraSpaces_ReturnsCollapsedString(string input, string output)
    {
        input.CollapseSpaces().ShouldBe(output);
    }

    public static IEnumerable<Func<(string, string)>> CollapseSpacesTestData()
    {
        yield return () => ("Hello World", "Hello World");
        yield return () => ("Hello World ", "Hello World");
        yield return () => ("Hello World  ", "Hello World");
        yield return () => (" Hello World", "Hello World");
        yield return () => ("  Hello World", "Hello World");
        yield return () => (" Hello   World   ", "Hello World");
    }

    [Test]
    [MethodDataSource(nameof(RemoveSpacesTestData))]
    public void RemoveSpaces_WithSpaces_ReturnsStringWithoutSpaces(string input, string output)
    {
        input.RemoveSpaces().ShouldBe(output);
    }

    public static IEnumerable<Func<(string, string)>> RemoveSpacesTestData()
    {
        yield return () => ("Hello World", "HelloWorld");
        yield return () => ("Hello World ", "HelloWorld");
        yield return () => ("Hello World  ", "HelloWorld");
        yield return () => (" Hello World", "HelloWorld");
        yield return () => ("  Hello World", "HelloWorld");
        yield return () => (" Hello   World   ", "HelloWorld");
    }
}
