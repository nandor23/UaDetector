using Shouldly;
using UaDetector.Abstractions.Enums;

namespace UaDetector.Abstractions.Tests.Enums;

public class ClientTypeTests
{
    [Test]
    public void ClientType_ValuesShouldBeSequential()
    {
        var values = Enum.GetValues<ClientType>().Cast<int>().ToList();

        for (int i = 0; i < values.Count; i++)
        {
            values[i].ShouldBe(i + 1);
        }
    }

    [Test]
    public void ClientType_HasExpectedValues()
    {
        var expectedValues = new Dictionary<ClientType, int>
        {
            { ClientType.MobileApp, 1 },
            { ClientType.MediaPlayer, 2 },
            { ClientType.Library, 3 },
            { ClientType.FeedReader, 4 },
            { ClientType.PersonalInformationManager, 5 },
        };

        expectedValues.Count.ShouldBe(Enum.GetValues<ClientType>().Length);

        Enum.GetValues<ClientType>().Length.ShouldBe(expectedValues.Count);

        foreach (var clientType in Enum.GetValues<ClientType>())
        {
            ((int)clientType).ShouldBe(expectedValues[clientType]);
        }
    }
}
