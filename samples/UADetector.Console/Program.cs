using UADetector.Models.Enums;
using UADetector.Parsers;

var parser = new BrowserParser(VersionTruncation.None);

var userAgent = "Mozilla/5.0 (Linux; U; Android 2.3.3; ja-jp; COOLPIX S800c Build/CP01_WW) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 Mobile Safari/533.1";

parser.TryParse(userAgent, out var result);
