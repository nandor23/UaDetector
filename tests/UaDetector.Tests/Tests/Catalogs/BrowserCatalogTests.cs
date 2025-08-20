using Shouldly;
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
}
