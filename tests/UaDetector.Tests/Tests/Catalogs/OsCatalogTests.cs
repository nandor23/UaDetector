using Shouldly;
using UaDetector.Abstractions.Constants;
using UaDetector.Abstractions.Enums;
using UaDetector.Catalogs;

namespace UaDetector.Tests.Tests.Catalogs;

public class OsCatalogTests
{
    [Test]
    public void OsCodeMapping_ShouldContainAllOsCodes()
    {
        foreach (OsCode osCode in Enum.GetValues<OsCode>())
        {
            OsCatalog.OsCodeMappings.ShouldContainKey(osCode);
        }
    }

    [Test]
    public void OsCodeMapping_ShouldContainUniqueValues()
    {
        OsCatalog.OsCodeMappings.Values.Length.ShouldBe(
            OsCatalog.OsCodeMappings.Values.Distinct().Count()
        );
    }

    [Test]
    public void GetOsName_ShouldReturnExpectedValue_ForValidOsCode()
    {
        OsCatalog.GetOsName(OsCode.Fedora).ShouldBe(OsNames.Fedora);
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
        var success = OsCatalog.TryGetOsCode(osName, out var actualOsCode);

        success.ShouldBe(expectedSuccess);
        actualOsCode.ShouldBe(expectedOsCode);
    }
}
