using Shouldly;
using UaDetector.Abstractions.Constants;
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

    [Test]
    public void GetBrandName_ShouldReturnExpectedValue_ForValidBrandCode()
    {
        BrandCatalog.GetBrandName(BrandCode.Dell).ShouldBe(BrandNames.Dell);
    }

    [Test]
    [Arguments(BrandNames.Apple, BrandCode.Apple, true)]
    [Arguments("", null, false)]
    public void TryGetBrandCode_WithValidAndInvalidNames_ReturnsExpectedResults(
        string brandName,
        BrandCode? expectedBrandCode,
        bool expectedSuccess
    )
    {
        var success = BrandCatalog.TryGetBrandCode(brandName, out var actualBrandCode);

        success.ShouldBe(expectedSuccess);
        actualBrandCode.ShouldBe(expectedBrandCode);
    }
}
