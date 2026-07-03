using Shouldly;
using UaDetector.Abstractions;
using UaDetector.Parsers;
using UaDetector.Parsers.Clients;
using UaDetector.Tests.Helpers;

namespace UaDetector.Tests.Tests.Parsers;

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

    [Test]
    public void TryParse_WhenCacheProvided_ShouldStoreResultInCache()
    {
        const string userAgent = "okhttp/3.12.1";
        var cache = new RecordingCache();
        var parser = new ClientParser(new UaDetectorOptions { Cache = cache });

        parser.TryParse(userAgent, out _);

        cache.SetKeys.ShouldContain(key => key.StartsWith("client:"));
    }

    [Test]
    public void TryParse_WhenCalledTwiceWithSameUserAgent_ShouldServeSecondCallFromCache()
    {
        const string userAgent = "okhttp/3.12.1";
        var cache = new RecordingCache();
        var parser = new ClientParser(new UaDetectorOptions { Cache = cache });

        parser.TryParse(userAgent, out _);
        int setsAfterFirstParse = cache.SetCount;

        parser.TryParse(userAgent, out _);

        cache.SetCount.ShouldBe(setsAfterFirstParse);
        cache.GetKeys.Count(key => key.StartsWith("client:")).ShouldBe(2);
    }
}
