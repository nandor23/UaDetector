using Shouldly;
using UaDetector.Abstractions.Constants;
using UaDetector.Abstractions.Enums;

namespace UaDetector.Tests.Tests.Catalogs;

public class BrandRegistryTests
{
    [Test]
    public void BrandCodeMapping_ShouldContainAllBrandCodes()
    {
        foreach (BrandCode brandCode in Enum.GetValues<BrandCode>())
        {
            BrandRegistry.BrandCodeMappings.ShouldContainKey(brandCode);
        }
    }

    [Test]
    public void BrandCodeMapping_ShouldContainUniqueValues()
    {
        BrandRegistry.BrandCodeMappings.Values.Length.ShouldBe(
            BrandRegistry.BrandCodeMappings.Values.Distinct().Count()
        );
    }

    [Test]
    public void GetBrandName_ShouldReturnExpectedValue_ForValidBrandCode()
    {
        BrandRegistry.GetBrandName(BrandCode.Dell).ShouldBe(BrandNames.Dell);
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
        var success = BrandRegistry.TryGetBrandCode(brandName, out var actualBrandCode);

        success.ShouldBe(expectedSuccess);
        actualBrandCode.ShouldBe(expectedBrandCode);
    }
}
