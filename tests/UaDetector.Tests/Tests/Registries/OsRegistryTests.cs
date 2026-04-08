using Shouldly;
using UaDetector.Abstractions.Constants;
using UaDetector.Abstractions.Enums;
using UaDetector.Registries;

namespace UaDetector.Tests.Tests.Registries;

public class OsRegistryTests
{
    [Test]
    public void OsCodeMapping_ShouldContainAllOsCodes()
    {
        foreach (OsCode osCode in Enum.GetValues<OsCode>())
        {
            OsRegistry.OsCodeMappings.ShouldContainKey(osCode);
        }
    }

    [Test]
    public void OsCodeMapping_ShouldContainUniqueValues()
    {
        OsRegistry.OsCodeMappings.Values.Length.ShouldBe(
            OsRegistry.OsCodeMappings.Values.Distinct().Count()
        );
    }

    [Test]
    [Arguments(OsCode.Mac, OsNames.Mac, true)]
    [Arguments(OsCode.Fedora, OsNames.Fedora, true)]
    [Arguments((OsCode)Int32.MaxValue, null, false)]
    public void TryGetOsName_ShouldReturnExpectedValue(
        OsCode osCode,
        string? expectedName,
        bool expectedSuccess
    )
    {
        var success = OsRegistry.TryGetOsName(osCode, out var actualOsName);

        success.ShouldBe(expectedSuccess);
        actualOsName.ShouldBe(expectedName);
    }

    [Test]
    [Arguments(OsNames.Mac, OsCode.Mac, true)]
    [Arguments(OsNames.Fedora, OsCode.Fedora, true)]
    [Arguments("", null, false)]
    public void TryGetOsCode_ShouldReturnExpectedValue(
        string osName,
        OsCode? expectedOsCode,
        bool expectedSuccess
    )
    {
        var success = OsRegistry.TryGetOsCode(osName, out var actualOsCode);

        success.ShouldBe(expectedSuccess);
        actualOsCode.ShouldBe(expectedOsCode);
    }
}
