using System.Collections.Immutable;

using Shouldly;

using UADetector.Models.Enums;
using UADetector.Parsers.Clients;
using UADetector.Tests.Fixtures.Models;
using UADetector.Tests.Helpers;

namespace UADetector.Tests.Tests.Parsers.Clients;

public class LibraryParserTests
{
    [Test]
    public void LibraryParser_Instantiation_ShouldNotThrowException()
    {
        Should.NotThrow(() => new LibraryParser(VersionTruncation.None));
    }

    [Test]
    public void LibraryParser_ShouldExtend_ClientParserBase()
    {
        var parser = new LibraryParser(VersionTruncation.None);
        parser.ShouldBeAssignableTo<ClientParserBase>();
    }

    [Test]
    public void TryParse_WithFixtureData_ShouldReturnExpectedClientInfo()
    {
        var fixturePath = Path.Combine("Fixtures", "Resources", "Clients", "libraries.yml");
        var fixtures = FixtureLoader.Load<ClientFixture>(fixturePath);

        var clientHints = ClientHints.Create(ImmutableDictionary<string, string?>.Empty);
        var parser = new LibraryParser(VersionTruncation.None);

        foreach (var fixture in fixtures)
        {
            parser.TryParse(fixture.UserAgent, clientHints, out var result).ShouldBeTrue();

            result.ShouldNotBeNull();
            result.Type.ShouldBe(ClientType.Library);
            result.Name.ShouldBe(fixture.Client.Name);
            result.Version.ShouldBe(fixture.Client.Version);
        }
    }
}
