using System.Collections.Immutable;

using Shouldly;

using UADetector.Parsers.Devices;
using UADetector.Tests.Fixtures.Models;
using UADetector.Tests.Helpers;

namespace UADetector.Tests.Tests.Parsers.Devices;

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
    public void TryParse_WithFixtureData_ShouldReturnCorrectInternalDeviceInfo()
    {
        var fixturePath = Path.Combine("Fixtures", "Resources", "Devices", "notebooks.yml");
        var fixtures = FixtureLoader.Load<DeviceFixture>(fixturePath);

        var clientHints = ClientHints.Create(ImmutableDictionary<string, string?>.Empty);
        var parser = new NotebookParser();

        foreach (var fixture in fixtures)
        {
            parser.TryParse(fixture.UserAgent, clientHints, out var result).ShouldBeTrue();

            result.ShouldNotBeNull();
            DeviceParserBase.DeviceTypeMapping.TryGetValue(fixture.Device.Type, out var deviceType).ShouldBeTrue();
            result.Type.ShouldBe(deviceType);
            result.Brand.ShouldBe(fixture.Device.Brand);
            result.Model.ShouldBe(fixture.Device.Model);
        }
    }
}
