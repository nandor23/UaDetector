using Shouldly;

using UADetector.Parsers.Devices;

namespace UADetector.Tests.Tests.Parsers.Devices;

public class MobileParserTests
{
    [Test]
    public void MobileParser_Instantiation_ShouldNotThrowException()
    {
        Should.NotThrow(() => new MobileParser());
    }

    [Test]
    public void MobileParser_ShouldExtend_DeviceParserBase()
    {
        var parser = new MobileParser();
        parser.ShouldBeAssignableTo<DeviceParserBase>();
    }
}
