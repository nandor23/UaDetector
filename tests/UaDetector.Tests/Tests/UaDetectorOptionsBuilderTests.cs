using Shouldly;
using UaDetector.Abstractions.Enums;

namespace UaDetector.Tests.Tests;

public class UaDetectorOptionsBuilderTests
{
    [Test]
    public void Build_ReturnsOptionsWithConfiguredValues()
    {
        var options = new UaDetectorOptionsBuilder
        {
            VersionTruncation = VersionTruncation.Major,
            DisableBotDetection = true,
        }.Build();

        options.VersionTruncation.ShouldBe(VersionTruncation.Major);
        options.DisableBotDetection.ShouldBeTrue();
    }
}
