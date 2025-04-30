using Shouldly;
using UaDetector.Models.Enums;

namespace UaDetector.Tests.Tests.Models.Enums;

public class VersionTruncationTests
{
    [Test]
    public void VersionTruncation_HasExpectedValues()
    {
        var expectedValues = new Dictionary<VersionTruncation, int>
        {
            { VersionTruncation.None, 0 },
            { VersionTruncation.Major, 1 },
            { VersionTruncation.Minor, 2 },
            { VersionTruncation.Patch, 3 },
            { VersionTruncation.Build, 4 },
        };

        Enum.GetValues<VersionTruncation>().Length.ShouldBe(expectedValues.Count);

        foreach (var versionTruncation in Enum.GetValues<VersionTruncation>())
        {
            ((int)versionTruncation).ShouldBe(expectedValues[versionTruncation]);
        }
    }
}
