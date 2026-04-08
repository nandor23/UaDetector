using Shouldly;
using UaDetector.Abstractions.Constants;
using UaDetector.Abstractions.Enums;
using UaDetector.Registries;

namespace UaDetector.Tests.Tests.Registries;

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
    [Arguments(BrowserCode.Brave, BrowserNames.Brave, true)]
    [Arguments(BrowserCode.TorBrowser, BrowserNames.TorBrowser, true)]
    [Arguments((BrowserCode)Int32.MaxValue, null, false)]
    public void TryGetBrowserName_ShouldReturnExpectedValue(
        BrowserCode browserCode,
        string? expectedBrowserName,
        bool expectedSuccess
    )
    {
        var success = BrowserRegistry.TryGetBrowserName(browserCode, out var actualBrowserName);

        success.ShouldBe(expectedSuccess);
        actualBrowserName.ShouldBe(expectedBrowserName);
    }

    [Test]
    [Arguments(BrowserNames.Brave, BrowserCode.Brave, true)]
    [Arguments(BrowserNames.TorBrowser, BrowserCode.TorBrowser, true)]
    [Arguments("", null, false)]
    public void TryGetBrowserCode_ShouldReturnExpectedValue(
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
