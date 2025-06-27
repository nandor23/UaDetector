using Shouldly;

using UaDetector.Parsers.Devices;

namespace UaDetector.Tests.Tests.Parsers.Devices;

public class PortableMediaPlayerParserTests
{
    [Test]
    public void PortableMediaPlayerParser_Instantiation_ShouldNotThrowException()
    {
        Should.NotThrow(() => new PortableMediaPlayerParser());
    }

    [Test]
    public void PortableMediaPlayerParser_ShouldExtend_DeviceParserBase()
    {
        var parser = new PortableMediaPlayerParser();
        parser.ShouldBeAssignableTo<DeviceParserBase>();
    }
}
