using UaDetector.Parsers;
using UaDetector.Registries;

namespace UaDetector.ReadmeUpdater.DataCollectors;

public class DeviceBrandDataCollector : IDataCollector
{
    public string MarkerName => "DEVICE-BRANDS";
    public string Title => "Device Brands";
    public string FileName => "device-brands.md";

    public IEnumerable<string> CollectData()
    {
        return BrandRegistry
            .BrandNameMappings.Keys.Concat(
                VendorFragmentParser.VendorFragments.Select(x => x.Brand)
            )
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }
}
