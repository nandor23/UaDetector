using Shouldly;

using UADetector.Parsers;
using UADetector.Parsers.Clients;

namespace UADetector.Tests.Tests.Parsers;

public class ClientParserTests
{
    [Test]
    public void ClientParser_Instantiation_ShouldNotThrowException()
    {
        Should.NotThrow(() => new ClientParser());
    }

    [Test]
    public void ClientParser_ShouldImplement_IClientParser()
    {
        var parser = new ClientParser();
        parser.ShouldBeAssignableTo<IClientParser>();
    }

    [Test]
    public void ClientParserCollection_ShouldIncludeAllClientParsers()
    {
        var parser = new ClientParser();

        parser.ClientParsers.Count().ShouldBe(5);
        parser.ClientParsers.OfType<MobileAppParser>().Any().ShouldBeTrue();
        parser.ClientParsers.OfType<MediaPlayerParser>().Any().ShouldBeTrue();
        parser.ClientParsers.OfType<LibraryParser>().Any().ShouldBeTrue();
        parser.ClientParsers.OfType<FeedReaderParser>().Any().ShouldBeTrue();
        parser.ClientParsers.OfType<PimParser>().Any().ShouldBeTrue();
    }
}
