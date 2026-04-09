using Shouldly;
using UaDetector.Abstractions.Enums;

namespace UaDetector.Abstractions.Tests.Enums;

public class DeviceTypeTests
{
    [Test]
    public void DeviceType_ValuesShouldBeSequential()
    {
        var values = Enum.GetValues<DeviceType>().Cast<int>().ToList();

        for (int i = 0; i < values.Count; i++)
        {
            values[i].ShouldBe(i + 1);
        }
    }

    [Test]
    public void DeviceType_HasExpectedValues()
    {
        var expectedValues = new Dictionary<DeviceType, int>
        {
            { DeviceType.Desktop, 1 },
            { DeviceType.Smartphone, 2 },
            { DeviceType.Tablet, 3 },
            { DeviceType.FeaturePhone, 4 },
            { DeviceType.Console, 5 },
            { DeviceType.Television, 6 },
            { DeviceType.CarBrowser, 7 },
            { DeviceType.SmartDisplay, 8 },
            { DeviceType.Camera, 9 },
            { DeviceType.PortableMediaPlayer, 10 },
            { DeviceType.Phablet, 11 },
            { DeviceType.SmartSpeaker, 12 },
            { DeviceType.Wearable, 13 },
            { DeviceType.Peripheral, 14 },
        };

        expectedValues.Count.ShouldBe(Enum.GetValues<DeviceType>().Length);

        foreach (var deviceType in Enum.GetValues<DeviceType>())
        {
            ((int)deviceType).ShouldBe(expectedValues[deviceType]);
        }
    }
}
