using Shouldly;

using UaDetector.Abstractions.Models.Enums;
using UaDetector.Parsers.Devices;

namespace UaDetector.Tests.Tests.Parsers.Devices;

public class DeviceParserBaseTests
{
    [Test]
    public void BrandCodeMapping_ShouldContainAllBrandCodes()
    {
        foreach (BrandCode brandCode in Enum.GetValues<BrandCode>())
        {
            DeviceParserBase.BrandCodeMapping.ShouldContainKey(brandCode);
        }
    }

    [Test]
    public void BrandCodeMapping_ShouldContainUniqueValues()
    {
        DeviceParserBase.BrandCodeMapping.Values.Length.ShouldBe(
            DeviceParserBase.BrandCodeMapping.Values.Distinct().Count()
        );
    }
}
