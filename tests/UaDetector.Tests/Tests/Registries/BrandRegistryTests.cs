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
    [Arguments(BrandCode.Apple, BrandNames.Apple, true)]
    [Arguments(BrandCode.Cloudpad, BrandNames.Cloudpad, true)]
    [Arguments((BrandCode)Int32.MaxValue, null, false)]
    public void TryGetBrandName_ShouldReturnExpectedValue(
        BrandCode brandCode,
        string? expectedBrandName,
        bool expectedSuccess
    )
    {
        var success = BrandRegistry.TryGetBrandName(brandCode, out var actualBrandName);

        success.ShouldBe(expectedSuccess);
        actualBrandName.ShouldBe(expectedBrandName);
    }

    [Test]
    [Arguments(BrandNames.Apple, BrandCode.Apple, true)]
    [Arguments(BrandNames.Cloudpad, BrandCode.Cloudpad, true)]
    [Arguments("", null, false)]
    public void TryGetBrandCode_ShouldReturnExpectedValue(
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
