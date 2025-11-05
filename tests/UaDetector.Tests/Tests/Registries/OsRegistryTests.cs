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
    public void GetOsName_ShouldReturnExpectedValue_ForValidOsCode()
    {
        OsRegistry.GetOsName(OsCode.Fedora).ShouldBe(OsNames.Fedora);
    }

    [Test]
    [Arguments(OsNames.Mac, OsCode.Mac, true)]
    [Arguments("", null, false)]
    public void TryGetOsCode_WithValidAndInvalidNames_ReturnsExpectedResults(
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
