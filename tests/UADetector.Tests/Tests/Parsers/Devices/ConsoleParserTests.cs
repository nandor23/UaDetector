using Shouldly;

using UADetector.Parsers.Devices;
using UADetector.Tests.Fixtures.Models;
using UADetector.Tests.Helpers;

namespace UADetector.Tests.Tests.Parsers.Devices;

public class ConsoleParserTests
{
    [Test]
    public void ConsoleParser_Instantiation_ShouldNotThrowException()
    {
        Should.NotThrow(() => new ConsoleParser());
    }

    [Test]
    public void ConsoleParser_ShouldExtend_DeviceParserBase()
    {
        var parser = new ConsoleParser();
        parser.ShouldBeAssignableTo<DeviceParserBase>();
    }

    [Test]
    public async Task TryParse_WithFixtureData_ShouldReturnExpectedInternalDeviceInfo()
    {
        var fixturePath = Path.Combine("Fixtures", "Resources", "Devices", "consoles.json");
        var fixtures = await FixtureLoader.LoadAsync<DeviceFixture>(fixturePath);
        var parser = new ConsoleParser();

        foreach (var fixture in fixtures)
        {
            parser.TryParse(fixture.UserAgent, out var result).ShouldBeTrue();

            result.ShouldNotBeNull();
            result.Type.ShouldBe(fixture.Device.Type);
            result.Brand.ShouldBe(fixture.Device.Brand);
            result.Model.ShouldBe(fixture.Device.Model);
        }
    }
}
