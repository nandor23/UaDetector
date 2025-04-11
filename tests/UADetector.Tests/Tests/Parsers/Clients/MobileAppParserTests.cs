using System.Collections.Immutable;

using Shouldly;

using UADetector.Models.Enums;
using UADetector.Parsers.Clients;
using UADetector.Tests.Fixtures.Models;
using UADetector.Tests.Helpers;

namespace UADetector.Tests.Tests.Parsers.Clients;

public class MobileAppParserTests
{
    [Test]
    public void MobileAppParser_Instantiation_ShouldNotThrowException()
    {
        Should.NotThrow(() => new MobileAppParser(VersionTruncation.None));
    }

    [Test]
    public void MobileAppParser_ShouldExtend_ClientParserBase()
    {
        var parser = new MobileAppParser(VersionTruncation.None);
        parser.ShouldBeAssignableTo<ClientParserBase>();
    }

    [Test]
    public async Task TryParse_WithFixtureData_ShouldReturnExpectedClientInfo()
    {
        var fixturePath = Path.Combine("Fixtures", "Resources", "Clients", "mobile_apps.json");
        var fixtures = await FixtureLoader.LoadAsync<ClientFixture>(fixturePath);

        var clientHints = ClientHints.Create(ImmutableDictionary<string, string?>.Empty);
        var parser = new MobileAppParser(VersionTruncation.None);

        foreach (var fixture in fixtures)
        {
            parser.TryParse(fixture.UserAgent, clientHints, out var result).ShouldBeTrue();

            result.ShouldNotBeNull();
            result.Type.ShouldBe(ClientType.MobileApp);
            result.Name.ShouldBe(fixture.Client.Name);
            result.Version.ShouldBe(fixture.Client.Version);
        }
    }
}
