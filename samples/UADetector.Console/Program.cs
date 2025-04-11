using UADetector.Models.Enums;
using UADetector.Parsers;

var parser = new BrowserParser(VersionTruncation.None);

var userAgent = "GoogleEarth/4.2.0184.9679(Windows;Microsoft Windows XP (Service Pack 2);en-US;kml:2.2;browser:Plus;type:default)";

parser.TryParse(userAgent, out var result);

Console.WriteLine(result);
