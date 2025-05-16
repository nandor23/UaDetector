using Shouldly;
using UaDetector.Parsers;

namespace UaDetector.Tests.Tests.Parsers;

public class ParserHelperTests
{
    [Test]
    [MethodDataSource(nameof(TryCompareVersionsValidTestData))]
    public void TryCompareVersions_WithValidInputs_ReturnsExpectedResult(
        string firstVersion,
        string secondVersion,
        int expectedResult
    )
    {
        var parserHelper = new ParserHelper();

        parserHelper.TryCompareVersions(firstVersion, secondVersion, out var result).ShouldBe(true);

        result.ShouldBe(expectedResult);
    }

    [Test]
    [MethodDataSource(nameof(TryCompareVersionsInvalidTestData))]
    public void TryCompareVersions_WithInvalidInputs_ReturnsExpectedResult(
        string firstVersion,
        string secondVersion
    )
    {
        var parserHelper = new ParserHelper();

        parserHelper
            .TryCompareVersions(firstVersion, secondVersion, out var result)
            .ShouldBe(false);

        result.ShouldBe(null);
    }

    public static IEnumerable<Func<(string, string, int)>> TryCompareVersionsValidTestData()
    {
        yield return () => ("1", "1", 0);
        yield return () => ("1", "1.0", 0);
        yield return () => ("1", "1.0.0", 0);
        yield return () => ("1.0", "1", 0);
        yield return () => ("1.0.0", "1", 0);
        yield return () => ("1.0", "1.1", -1);
        yield return () => ("1.0.1", "1.0.2", -1);
        yield return () => ("1.1", "1.0.0", 1);
        yield return () => ("1.0.20", "1.0.2", 1);
    }

    public static IEnumerable<Func<(string, string)>> TryCompareVersionsInvalidTestData()
    {
        yield return () => ("1", "abc");
        yield return () => ("1", string.Empty);
        yield return () => (string.Empty, "1.3");
        yield return () => ("abc", "1");
        yield return () => ("abc", "ab");
        yield return () => (string.Empty, string.Empty);
    }
}
