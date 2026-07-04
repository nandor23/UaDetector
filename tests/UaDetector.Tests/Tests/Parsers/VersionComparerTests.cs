using Shouldly;
using UaDetector.Parsers;

namespace UaDetector.Tests.Tests.Parsers;

public class VersionComparerTests
{
    [Test]
    [MethodDataSource(nameof(ComparableVersionData))]
    public void VersionComparer_WithComparableVersions_ReturnsExpectedResult(
        string first,
        string second,
        int sign
    )
    {
        VersionComparer.IsLessThan(first, second).ShouldBe(sign < 0);
        VersionComparer.IsLessThanOrEqual(first, second).ShouldBe(sign <= 0);
        VersionComparer.IsGreaterThan(first, second).ShouldBe(sign > 0);
        VersionComparer.IsGreaterThanOrEqual(first, second).ShouldBe(sign >= 0);
        VersionComparer.AreEqual(first, second).ShouldBe(sign == 0);
    }

    [Test]
    [MethodDataSource(nameof(NotComparableVersionData))]
    public void VersionComparer_WithNullOrEmptyVersion_AlwaysReturnsFalse(
        string? first,
        string? second
    )
    {
        // A null or empty version is not comparable, so every comparison returns false.
        VersionComparer.IsLessThan(first, second).ShouldBeFalse();
        VersionComparer.IsLessThanOrEqual(first, second).ShouldBeFalse();
        VersionComparer.IsGreaterThan(first, second).ShouldBeFalse();
        VersionComparer.IsGreaterThanOrEqual(first, second).ShouldBeFalse();
        VersionComparer.AreEqual(first, second).ShouldBeFalse();
    }

    // sign < 0 => first is lower, 0 => equal, > 0 => first is greater.
    public static IEnumerable<(string first, string second, int sign)> ComparableVersionData()
    {
        // Single and multi-segment numeric comparisons.
        yield return ("1.0", "1.1", -1);
        yield return ("1.1", "1.1", 0);
        yield return ("2.0", "1.9", 1);
        yield return ("1.2.3.4", "1.2.3.5", -1);
        yield return ("1.2.3.4", "1.2.3.4", 0);
        yield return ("10.0", "9.9.9", 1);

        // Segments compare by value, not lexically ("9" < "10").
        yield return ("1.9", "1.10", -1);
        yield return ("1.10", "1.9", 1);
        yield return ("142.0.7444.171", "142.0.7444.170", 1);

        // A longer version outranks a shorter one that shares its prefix.
        yield return ("1.0", "1", 1);
        yield return ("1", "1.0", -1);
        yield return ("1.2.0", "1.2", 1);
        yield return ("1.2", "1.2.0", -1);

        // Zero segments.
        yield return ("0", "0", 0);
        yield return ("0.0", "0", 1);

        // A numeric segment outranks a non-numeric one (named OS versions).
        yield return ("XP", "8", -1);
        yield return ("8", "XP", 1);
        yield return ("Vista", "8", -1);
        yield return ("10", "Vista", 1);

        // A named version must not be treated as equal to zero.
        yield return ("Vista", "0", -1);

        // Two non-numeric segments fall back to an ordinal comparison.
        yield return ("XP", "XP", 0);
        yield return ("Vista", "XP", -1);
        yield return ("Server 2012 R2", "Server 2012 R2", 0);

        // Non-numeric segment nested inside an otherwise numeric version.
        yield return ("10.0.beta", "10.0.1", -1);
        yield return ("10.0.1", "10.0.beta", 1);
        yield return ("18.11.0.350.01 beta", "18.11.0.350.02 alpha", -1);

        // Real thresholds used by the parsers (Android device type, Windows 8+).
        yield return ("1.5", "2.0", -1);
        yield return ("3.2", "4.0", -1);
        yield return ("3.0", "3.0", 0);
        yield return ("14", "8", 1);
    }

    public static IEnumerable<(string? first, string? second)> NotComparableVersionData()
    {
        yield return ("", "1");
        yield return ("1", "");
        yield return ("", "");
        yield return (null, "1");
        yield return ("1", null);
        yield return (null, null);
    }
}
