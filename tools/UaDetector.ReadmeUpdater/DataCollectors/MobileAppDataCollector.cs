using UaDetector.Parsers.Clients;

namespace UaDetector.ReadmeUpdater.DataCollectors;

public class MobileAppDataCollector : IDataCollector
{
    public string MarkerName => "MOBILE-APPS";
    public string Title => "Mobile Apps";
    public string FileName => "mobile-apps.md";

    public IEnumerable<string> CollectData()
    {
        return MobileAppParser.MobileApps
            .Select(app => app.Name)
            .Concat(AppHintParser.Hints.Values)
            .Concat(GetAdditionalApps())
            .Where(name => !name.Contains("$1") && !name.Contains("$2"))
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }

    private static IEnumerable<string> GetAdditionalApps()
    {
        return new List<string>
        {
            // Microsoft Office $1
            "Microsoft Office Access",
            "Microsoft Office Excel",
            "Microsoft Office OneDrive for Business",
            "Microsoft Office OneNote",
            "Microsoft Office PowerPoint",
            "Microsoft Office Project",
            "Microsoft Office Publisher",
            "Microsoft Office Visio",
            "Microsoft Office Word",
            // Podkicker$1
            "Podkicker",
            "Podkicker Pro",
            "Podkicker Classic",
            // radio.$1
            "radio.at",
            "radio.de",
            "radio.dk",
            "radio.es",
            "radio.fr",
            "radio.it",
            "radio.pl",
            "radio.pt",
            "radio.se",
            "radio.net"
        };
    }
}
