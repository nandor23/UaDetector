using Shouldly;

using UADetector.Parsers;
using UADetector.Tests.Fixtures.Models;
using UADetector.Tests.Helpers;

namespace UADetector.Tests.Tests.Parsers;

public sealed class VendorFragmentParserTests
{
    [Test]
    public void TryParse_WithFixtureData_ShouldReturnCorrectOsInfo()
    {
        var fixturePath = Path.Combine("Fixtures", "Resources", "vendor_fragments.yml");
        var fixtures = FixtureLoader.Load<VendorFragmentFixture>(fixturePath);

        foreach (var fixture in fixtures)
        {
            VendorFragmentParser.TryParseBrand(fixture.UserAgent, out string? result).ShouldBeTrue();

            result.ShouldNotBeNull();
            result.ShouldBe(fixture.Vendor);
        }
    }
}
