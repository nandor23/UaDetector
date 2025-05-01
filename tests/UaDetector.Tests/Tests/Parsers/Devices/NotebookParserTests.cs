using Shouldly;

using UaDetector.Parsers.Devices;
using UaDetector.Tests.Fixtures.Models;
using UaDetector.Tests.Helpers;

namespace UaDetector.Tests.Tests.Parsers.Devices;

public class NotebookParserTests
{
    [Test]
    public void NotebookParser_Instantiation_ShouldNotThrowException()
    {
        Should.NotThrow(() => new NotebookParser());
    }

    [Test]
    public void NotebookParser_ShouldExtend_DeviceParserBase()
    {
        var parser = new NotebookParser();
        parser.ShouldBeAssignableTo<DeviceParserBase>();
    }

    [Test]
    public async Task TryParse_WithFixtureData_ShouldReturnExpectedInternalDeviceInfo()
    {
        var fixturePath = Path.Combine("Fixtures", "Resources", "Devices", "notebooks.json");
        var fixtures = await FixtureLoader.LoadAsync<DeviceFixture>(fixturePath);

        var parser = new NotebookParser();

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
