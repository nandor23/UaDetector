using Shouldly;

using UADetector.Models.Enums;
using UADetector.Parsers.Devices;

namespace UADetector.Tests.Tests.Parsers.Devices;

public class DeviceParserBaseTests
{
    [Test]
    public void BrandCodeMapping_ShouldContainAllBrandCodes()
    {
        foreach (BrandCode brandCode in Enum.GetValues(typeof(BrandCode)))
        {
            DeviceParserBase.BrandCodeMapping.ShouldContainKey(brandCode);
        }
    }

    [Test]
    public void DeviceTypeMapping_ShouldContainAllDeviceTypes()
    {
        foreach (DeviceType deviceType in Enum.GetValues(typeof(DeviceType)))
        {
            DeviceParserBase.DeviceTypeMapping.Values.ShouldContain(deviceType);
        }
    }
}
