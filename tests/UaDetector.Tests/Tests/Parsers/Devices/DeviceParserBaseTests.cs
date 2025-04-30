using Shouldly;
using TUnit.Core;
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

    [Test]
    public void DeviceTypeMapping_ShouldContainAllDeviceTypes()
    {
        foreach (DeviceType deviceType in Enum.GetValues<DeviceType>())
        {
            DeviceParserBase.DeviceTypeMapping.Values.ShouldContain(deviceType);
        }
    }

    [Test]
    [MethodDataSource(nameof(DeviceTypeTestData))]
    public void DeviceTypeMapping_ShouldContainKey(string deviceType)
    {
        DeviceParserBase.DeviceTypeMapping.ShouldContainKey(deviceType);
    }

    public static IEnumerable<Func<string>> DeviceTypeTestData()
    {
        yield return () => "desktop";
        yield return () => "smartphone";
        yield return () => "tablet";
        yield return () => "feature phone";
        yield return () => "console";
        yield return () => "tv";
        yield return () => "car browser";
        yield return () => "smart display";
        yield return () => "camera";
        yield return () => "portable media player";
        yield return () => "phablet";
        yield return () => "smart speaker";
        yield return () => "wearable";
        yield return () => "peripheral";
    }
}
