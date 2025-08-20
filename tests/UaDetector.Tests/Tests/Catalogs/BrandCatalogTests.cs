using Shouldly;
using UaDetector.Abstractions.Enums;
using UaDetector.Catalogs;

namespace UaDetector.Tests.Tests.Catalogs;

public class BrandCatalogTests
{
    [Test]
    public void BrandCodeMapping_ShouldContainAllBrandCodes()
    {
        foreach (BrandCode brandCode in Enum.GetValues<BrandCode>())
        {
            BrandCatalog.BrandCodeMappings.ShouldContainKey(brandCode);
        }
    }

    [Test]
    public void BrandCodeMapping_ShouldContainUniqueValues()
    {
        BrandCatalog.BrandCodeMappings.Values.Length.ShouldBe(
            BrandCatalog.BrandCodeMappings.Values.Distinct().Count()
        );
    }
}
