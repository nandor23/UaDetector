using Shouldly;

using UaDetector.Abstractions.Models.Enums;

namespace UaDetector.Tests.Tests.Models.Enums;

public class VersionTruncationTests
{
    [Test]
    public void VersionTruncation_ValuesShouldBeSequential()
    {
        var values = Enum.GetValues<VersionTruncation>().Cast<int>().ToList();

        for (int i = 0; i < values.Count; i++)
        {
            values[i].ShouldBe(i);
        }
    }

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

        expectedValues.Count.ShouldBe(Enum.GetValues<VersionTruncation>().Length);

        foreach (var versionTruncation in Enum.GetValues<VersionTruncation>())
        {
            ((int)versionTruncation).ShouldBe(expectedValues[versionTruncation]);
        }
    }
}
