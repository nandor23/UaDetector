using UaDetector.ReadmeUpdater.DataCollectors;
using UaDetector.ReadmeUpdater.Generators;

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
    new BotDataCollector()
};

var readmeGenerator = new ReadmeGenerator();
readmeGenerator.Update(collectors);

var docsGenerator = new DocsGenerator();
docsGenerator.Generate(collectors);

Console.WriteLine("\nAll updates completed successfully!");
