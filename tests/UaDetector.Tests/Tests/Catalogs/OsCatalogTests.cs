using Shouldly;
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
}
