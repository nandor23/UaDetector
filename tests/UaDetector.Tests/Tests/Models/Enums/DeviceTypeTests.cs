using Shouldly;
using UaDetector.Models.Enums;

namespace UaDetector.Tests.Tests.Models.Enums;

public class DeviceTypeTests
{
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
            { DeviceType.Tv, 5 },
            { DeviceType.CarBrowser, 6 },
            { DeviceType.SmartDisplay, 7 },
            { DeviceType.Camera, 8 },
            { DeviceType.PortableMediaPlayer, 9 },
            { DeviceType.Phablet, 10 },
            { DeviceType.SmartSpeaker, 11 },
            { DeviceType.Wearable, 12 },
            { DeviceType.Peripheral, 13 },
        };

        Enum.GetValues<DeviceType>().Length.ShouldBe(expectedValues.Count);

        foreach (var deviceType in Enum.GetValues<DeviceType>())
        {
            ((int)deviceType).ShouldBe(expectedValues[deviceType]);
        }
    }
}
