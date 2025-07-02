using UaDetector.Parsers;
using UaDetector.Parsers.Browsers;
using UaDetector.Parsers.Clients;
using UaDetector.Parsers.Devices;
using UaDetector.ReadmeUpdater;

var readmePath = ReadmeLocator.GetReadmePath();
var originalReadme = File.ReadAllText(readmePath);

var browsers = BrowserParser
    .BrowserNameMapping.Keys.Concat(BrowserHintParser.Hints.Values)
    .Distinct(StringComparer.OrdinalIgnoreCase);

var mobileApps = MobileAppParser
    .MobileApps.Select(app => app.Name)
    .Concat(AppHintParser.Hints.Values)
    .Concat(
        new List<string>
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
            "radio.net",
        }
    )
    .Where(name => !name.Contains("$1"))
    .Distinct(StringComparer.OrdinalIgnoreCase);

var deviceBrands = DeviceParserBase
    .BrandNameMapping.Keys.Concat(VendorFragmentParser.VendorFragments.Select(x => x.Brand))
    .Distinct(StringComparer.OrdinalIgnoreCase);

var modifiedReadme = originalReadme
    .ReplaceMarkerContent("OPERATING-SYSTEMS", OsParser.OsNameMapping.Keys)
    .ReplaceMarkerContent("BROWSERS", browsers)
    .ReplaceMarkerContent("BROWSER-ENGINES", EngineParser.EngineNames)
    .ReplaceMarkerContent("MOBILE-APPS", mobileApps)
    .ReplaceMarkerContent("MEDIA-PLAYERS", MediaPlayerParser.MediaPlayers.Select(x => x.Name))
    .ReplaceMarkerContent("LIBRARIES", LibraryParser.Libraries.Select(x => x.Name))
    .ReplaceMarkerContent("FEED-READERS", FeedReaderParser.FeedReaders.Select(x => x.Name))
    .ReplaceMarkerContent(
        "PERSONAL-INFORMATION-MANAGERS",
        PimParser.PersonalInformationManagers.Select(x => x.Name)
    )
    .ReplaceMarkerContent("MEDIA-PLAYERS", MediaPlayerParser.MediaPlayers.Select(x => x.Name))
    .ReplaceMarkerContent("DEVICE-BRANDS", deviceBrands)
    .ReplaceMarkerContent("BOTS", BotParser.Bots.Select(x => x.Name));

if (originalReadme != modifiedReadme)
{
    modifiedReadme = modifiedReadme.ReplaceMarkerContent(
        "LAST-UPDATED",
        DateTime.Today.ToString("yyyy-MM-dd")
    );
}

File.WriteAllText(readmePath, modifiedReadme);
