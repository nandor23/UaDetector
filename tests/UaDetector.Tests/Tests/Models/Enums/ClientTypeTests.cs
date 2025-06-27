using Shouldly;

using UaDetector.Models.Enums;

namespace UaDetector.Tests.Tests.Models.Enums;

public class ClientTypeTests
{
    [Test]
    public void ClientType_ValuesShouldBeSequential()
    {
        var values = Enum.GetValues<ClientType>().Cast<int>().ToList();

        for (int i = 0; i < values.Count; i++)
        {
            values[i].ShouldBe(i);
        }
    }

    [Test]
    public void ClientType_HasExpectedValues()
    {
        var expectedValues = new Dictionary<ClientType, int>
        {
            { ClientType.MobileApp, 0 },
            { ClientType.MediaPlayer, 1 },
            { ClientType.Library, 2 },
            { ClientType.FeedReader, 3 },
            { ClientType.PersonalInformationManager, 4 },
        };

        expectedValues.Count.ShouldBe(Enum.GetValues<ClientType>().Length);

        Enum.GetValues<ClientType>().Length.ShouldBe(expectedValues.Count);

        foreach (var clientType in Enum.GetValues<ClientType>())
        {
            ((int)clientType).ShouldBe(expectedValues[clientType]);
        }
    }
}
