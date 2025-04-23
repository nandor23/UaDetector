using UaDetector.Parsers;
using UaDetector.Parsers.Browsers;
using UaDetector.Parsers.Clients;
using UaDetector.ReadmeUpdater;

Console.WriteLine("Hello, World!");

var readmePath = ReadmeLocator.GetReadmePath();
var readmeText = File.ReadAllText(readmePath);

var updatedReadme = readmeText.ReplaceBetweenMarkers("OPERATING-SYSTEMS", OsParser.OsNameMapping.Keys);

var browsers = BrowserParser.BrowserNameMapping.Keys
    .Concat(BrowserHintParser.Hints.Values)
    .Distinct(StringComparer.OrdinalIgnoreCase);

updatedReadme = updatedReadme.ReplaceBetweenMarkers("BROWSERS", browsers);
updatedReadme = updatedReadme.ReplaceBetweenMarkers("BROWSER-ENGINES", EngineParser.EngineNames);

var mobileAppNames = new List<string>
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
};

var mobileApps = MobileAppParser.MobileApps.Select(app => app.Name)
    .Concat(AppHintParser.Hints.Values)
    .Concat(mobileAppNames)
    .Where(name => !name.Contains("$1"))
    .Distinct(StringComparer.OrdinalIgnoreCase);

updatedReadme = updatedReadme.ReplaceBetweenMarkers("MOBILE-APPS", mobileApps);

updatedReadme = updatedReadme.ReplaceBetweenMarkers(
    "MEDIA-PLAYERS",
    MediaPlayerParser.MediaPlayers.Select(x => x.Name)
);





// Compare initial and modified text prior ot this
updatedReadme = updatedReadme.ReplaceBetweenMarkers("LAST-UPDATED", DateTime.Today.ToString("yyyy-MM-dd"));



File.WriteAllText(readmePath, updatedReadme);

Console.WriteLine(updatedReadme);
