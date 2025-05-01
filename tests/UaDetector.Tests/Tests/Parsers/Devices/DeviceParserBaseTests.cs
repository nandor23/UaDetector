using Shouldly;
using UaDetector.Models.Enums;
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
}
