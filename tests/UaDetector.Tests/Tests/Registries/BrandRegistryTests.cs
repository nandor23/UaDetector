using Shouldly;
using UaDetector.Abstractions.Constants;
using UaDetector.Abstractions.Enums;
using UaDetector.Registries;

namespace UaDetector.Tests.Tests.Registries;

public class BrandRegistryTests
{
    [Test]
    public void BrandCodeMapping_ShouldContainAllBrandCodes()
    {
        foreach (BrandCode brandCode in Enum.GetValues<BrandCode>())
        {
            BrandRegistry.ContainsCode(brandCode).ShouldBeTrue();
        }
    }

    [Test]
    public void BrandCodeMapping_ShouldContainUniqueValues()
    {
        BrandRegistry.BrandCodeMappings.Values.Length.ShouldBe(
            BrandRegistry.BrandCodeMappings.Values.Distinct().Count()
        );

        BrandRegistry.LegacyBrandCodeMappings.Values.Length.ShouldBe(
            BrandRegistry.LegacyBrandCodeMappings.Values.Distinct().Count()
        );
    }

    [Test]
    [Arguments(BrandCode.Apple, BrandNames.Apple)]
    [Arguments(BrandCode.Cloudpad, BrandNames.Cloudpad)]
    public void GetBrandName_ShouldReturnExpectedValue_ForValidBrandCode(
        BrandCode brandCode,
        string expectedBrandName
    )
    {
        var actualBrandName = BrandRegistry.GetBrandName(brandCode);
        actualBrandName.ShouldBe(expectedBrandName);
    }

    [Test]
    [Arguments(BrandNames.Apple, BrandCode.Apple, true)]
    [Arguments(BrandNames.Cloudpad, BrandCode.Cloudpad, true)]
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
