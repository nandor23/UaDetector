using System.Collections.Immutable;

using Shouldly;

using UADetector.Models.Enums;
using UADetector.Parsers.Devices;
using UADetector.Tests.Fixtures.Models;
using UADetector.Tests.Helpers;

namespace UADetector.Tests.Tests.Parsers.Devices;

public class CameraParserTests
{
    [Test]
    public void CameraParser_Instantiation_ShouldNotThrowException()
    {
        Should.NotThrow(() => new CameraParser());
    }

    [Test]
    public void CameraParser_ShouldExtend_DeviceParserBase()
    {
        var parser = new CameraParser();
        parser.ShouldBeAssignableTo<DeviceParserBase>();
    }

    [Test]
    public void TryParse_WithFixtureData_ShouldReturnExpectedInternalDeviceInfo()
    {
        var fixturePath = Path.Combine("Fixtures", "Resources", "Devices", "cameras.yml");
        var fixtures = FixtureLoader.Load<DeviceFixture>(fixturePath);

        var clientHints = ClientHints.Create(ImmutableDictionary<string, string?>.Empty);
        var parser = new CameraParser();

        foreach (var fixture in fixtures)
        {
            parser.TryParse(fixture.UserAgent, clientHints, out var result).ShouldBeTrue();

            result.ShouldNotBeNull();
            result.Type.ShouldBe(DeviceType.Camera);
            result.Brand.ShouldBe(fixture.Device.Brand);
            result.Model.ShouldBe(fixture.Device.Model);
        }
    }
}
