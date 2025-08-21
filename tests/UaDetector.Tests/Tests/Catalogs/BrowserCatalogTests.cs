using Shouldly;
using UaDetector.Abstractions.Constants;
using UaDetector.Abstractions.Enums;
using UaDetector.Catalogs;

namespace UaDetector.Tests.Tests.Catalogs;

public class BrowserCatalogTests
{
    [Test]
    public void BrowserCodeMapping_ShouldContainAllBrowserCodes()
    {
        foreach (BrowserCode browserCode in Enum.GetValues<BrowserCode>())
        {
            BrowserCatalog.BrowserCodeMappings.ShouldContainKey(browserCode);
        }
    }

    [Test]
    public void BrowserCodeMapping_ShouldContainUniqueValues()
    {
        BrowserCatalog.BrowserCodeMappings.Values.Length.ShouldBe(
            BrowserCatalog.BrowserCodeMappings.Values.Distinct().Count()
        );
    }

    [Test]
    public void GetBrowserName_ShouldReturnExpectedValue_ForValidBrowserCode()
    {
        BrowserCatalog.GetBrowserName(BrowserCode.Safari).ShouldBe(BrowserNames.Safari);
    }

    [Test]
    [Arguments(BrowserNames.Brave, BrowserCode.Brave, true)]
    [Arguments("", null, false)]
    public void TryGetBrowserCode_WithValidAndInvalidNames_ReturnsExpectedResults(
        string browserName,
        BrowserCode? expectedBrowserCode,
        bool expectedSuccess
    )
    {
        var success = BrowserCatalog.TryGetBrowserCode(browserName, out var actualBrowserCode);

        success.ShouldBe(expectedSuccess);
        actualBrowserCode.ShouldBe(expectedBrowserCode);
    }
}
