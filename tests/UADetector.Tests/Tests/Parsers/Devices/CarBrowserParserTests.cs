using System.Collections.Immutable;

using Shouldly;

using UADetector.Models.Enums;
using UADetector.Parsers.Devices;
using UADetector.Tests.Fixtures.Models;
using UADetector.Tests.Helpers;

namespace UADetector.Tests.Tests.Parsers.Devices;

public class CarBrowserParserTests
{
    [Test]
    public void CarBrowserParser_Instantiation_ShouldNotThrowException()
    {
        Should.NotThrow(() => new CarBrowserParser());
    }

    [Test]
    public void CarBrowserParser_ShouldExtend_DeviceParserBase()
    {
        var parser = new CarBrowserParser();
        parser.ShouldBeAssignableTo<DeviceParserBase>();
    }

    [Test]
    public void TryParse_WithFixtureData_ShouldReturnExpectedInternalDeviceInfo()
    {
        var fixturePath = Path.Combine("Fixtures", "Resources", "Devices", "car_browsers.yml");
        var fixtures = FixtureLoader.Load<DeviceFixture>(fixturePath);

        var clientHints = ClientHints.Create(ImmutableDictionary<string, string?>.Empty);
        var parser = new CarBrowserParser();

        foreach (var fixture in fixtures)
        {
            parser.TryParse(fixture.UserAgent, clientHints, out var result).ShouldBeTrue();

            result.ShouldNotBeNull();
            result.Type.ShouldBe(DeviceType.CarBrowser);
            result.Brand.ShouldBe(fixture.Device.Brand);
            result.Model.ShouldBe(fixture.Device.Model);
        }
    }
}
