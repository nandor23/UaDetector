using System.Collections.Immutable;
using Shouldly;

using TUnit.Core;

using UaDetector.Models.Enums;
using UaDetector.Parsers.Clients;
using UaDetector.Tests.Fixtures.Models;
using UaDetector.Tests.Helpers;

namespace UaDetector.Tests.Tests.Parsers.Clients;

public class FeedReaderParserTests
{
    [Test]
    public void FeedReaderParser_Instantiation_ShouldNotThrowException()
    {
        Should.NotThrow(() => new FeedReaderParser(VersionTruncation.None));
    }

    [Test]
    public void FeedReaderParser_ShouldExtend_ClientParserBase()
    {
        var parser = new FeedReaderParser(VersionTruncation.None);
        parser.ShouldBeAssignableTo<ClientParserBase>();
    }

    [Test]
    public async Task TryParse_WithFixtureData_ShouldReturnExpectedClientInfo()
    {
        var fixturePath = Path.Combine("Fixtures", "Resources", "Clients", "feed_readers.json");
        var fixtures = await FixtureLoader.LoadAsync<ClientFixture>(fixturePath);

        var clientHints = ClientHints.Create(ImmutableDictionary<string, string?>.Empty);
        var parser = new FeedReaderParser(VersionTruncation.None);

        foreach (var fixture in fixtures)
        {
            parser.TryParse(fixture.UserAgent, clientHints, out var result).ShouldBeTrue();

            result.ShouldNotBeNull();
            result.Type.ShouldBe(ClientType.FeedReader);
            result.Name.ShouldBe(fixture.Client.Name);
            result.Version.ShouldBe(fixture.Client.Version);
        }
    }
}
