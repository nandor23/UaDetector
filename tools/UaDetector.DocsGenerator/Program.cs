using UaDetector.DocsGenerator.DataCollectors;
using UaDetector.DocsGenerator.Generators;

var collectors = new List<IDataCollector>
{
    new OsDataCollector(),
    new BrowserDataCollector(),
    new BrowserEngineDataCollector(),
    new MobileAppDataCollector(),
    new MediaPlayerDataCollector(),
    new LibraryDataCollector(),
    new FeedReaderDataCollector(),
    new PimDataCollector(),
    new DeviceBrandDataCollector(),
    new BotDataCollector(),
};

ReadmeGenerator.Update(collectors);

DocsGenerator.Generate(collectors);

Console.WriteLine("\nAll updates completed successfully!");
