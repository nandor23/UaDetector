using Shouldly;

using UaDetector.Models.Enums;

namespace UaDetector.Tests.Tests.Models.Enums;

public class DeviceTypeTests
{
    [Test]
    public void DeviceType_ValuesShouldBeSequential()
    {
        var values = Enum.GetValues<DeviceType>().Cast<int>().ToList();

        for (int i = 0; i < values.Count; i++)
        {
            values[i].ShouldBe(i);
        }
    }

    [Test]
    public void DeviceType_HasExpectedValues()
    {
        var expectedValues = new Dictionary<DeviceType, int>
        {
            { DeviceType.Desktop, 0 },
            { DeviceType.Smartphone, 1 },
            { DeviceType.Tablet, 2 },
            { DeviceType.FeaturePhone, 3 },
            { DeviceType.Console, 4 },
            { DeviceType.Television, 5 },
            { DeviceType.CarBrowser, 6 },
            { DeviceType.SmartDisplay, 7 },
            { DeviceType.Camera, 8 },
            { DeviceType.PortableMediaPlayer, 9 },
            { DeviceType.Phablet, 10 },
            { DeviceType.SmartSpeaker, 11 },
            { DeviceType.Wearable, 12 },
            { DeviceType.Peripheral, 13 },
        };

        expectedValues.Count.ShouldBe(Enum.GetValues<DeviceType>().Length);

        foreach (var deviceType in Enum.GetValues<DeviceType>())
        {
            ((int)deviceType).ShouldBe(expectedValues[deviceType]);
        }
    }
}
