using Shouldly;
using UaDetector.Parsers;

namespace UaDetector.Tests.Tests.Parsers;

public class VersionComparerTests
{
    [Test]
    public void VersionComparer_ReturnsExpectedResult()
    {
        VersionComparer.IsLessThan("1.0", "1.1").ShouldBeTrue();
        VersionComparer.IsLessThan("1.1", "1.1").ShouldBeFalse();

        VersionComparer.IsLessThanOrEqual("1.0", "1.0").ShouldBeTrue();
        VersionComparer.IsLessThanOrEqual("1.2", "1.1").ShouldBeFalse();

        VersionComparer.IsGreaterThan("2.0", "1.9").ShouldBeTrue();
        VersionComparer.IsGreaterThan("1.0", "1.0").ShouldBeFalse();

        VersionComparer.IsGreaterThanOrEqual("1.0", "1.0").ShouldBeTrue();
        VersionComparer.IsGreaterThanOrEqual("1.0", "1.1").ShouldBeFalse();

        VersionComparer.AreEqual("1.0", "1.0").ShouldBeTrue();

        // A trailing segment ranks the longer version higher.
        VersionComparer.AreEqual("1.0", "1").ShouldBeFalse();
        VersionComparer.IsGreaterThan("1.0", "1").ShouldBeTrue();

        // A null or empty version is not comparable, so every comparison returns false.
        VersionComparer.IsLessThan("", "1").ShouldBeFalse();
        VersionComparer.IsGreaterThan("1", "").ShouldBeFalse();
        VersionComparer.AreEqual("", "").ShouldBeFalse();
        VersionComparer.IsLessThan(null, "1").ShouldBeFalse();
    }
}
