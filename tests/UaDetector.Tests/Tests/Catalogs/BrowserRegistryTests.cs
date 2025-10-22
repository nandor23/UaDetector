using Shouldly;
using UaDetector.Abstractions.Constants;
using UaDetector.Abstractions.Enums;
using UaDetector.Registries;

namespace UaDetector.Tests.Tests.Catalogs;

public class BrowserRegistryTests
{
    [Test]
    public void BrowserCodeMapping_ShouldContainAllBrowserCodes()
    {
        foreach (BrowserCode browserCode in Enum.GetValues<BrowserCode>())
        {
            BrowserRegistry.BrowserCodeMappings.ShouldContainKey(browserCode);
        }
    }

    [Test]
    public void BrowserCodeMapping_ShouldContainUniqueValues()
    {
        BrowserRegistry.BrowserCodeMappings.Values.Length.ShouldBe(
            BrowserRegistry.BrowserCodeMappings.Values.Distinct().Count()
        );
    }

    [Test]
    public void GetBrowserName_ShouldReturnExpectedValue_ForValidBrowserCode()
    {
        BrowserRegistry.GetBrowserName(BrowserCode.Safari).ShouldBe(BrowserNames.Safari);
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
        var success = BrowserRegistry.TryGetBrowserCode(browserName, out var actualBrowserCode);

        success.ShouldBe(expectedSuccess);
        actualBrowserCode.ShouldBe(expectedBrowserCode);
    }
}
