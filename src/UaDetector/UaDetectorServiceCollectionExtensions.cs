using Microsoft.Extensions.DependencyInjection;
using UaDetector.Parsers;

namespace UaDetector;

public static class UaDetectorServiceCollectionExtensions
{
    private const string WarmupUserAgent = "uadetector-warmup";

    public static IServiceCollection AddUaDetector(
        this IServiceCollection services,
        Action<UaDetectorOptionsBuilder>? optionsAction = null
    )
    {
        var optionsBuilder = new UaDetectorOptionsBuilder();
        optionsAction?.Invoke(optionsBuilder);

        var uaDetector = new UaDetector(optionsBuilder.Build());

        services.AddSingleton<IUaDetector>(uaDetector);

        _ = Task.Run(() => uaDetector.TryParse(WarmupUserAgent, out _));

        return services;
    }

    public static IServiceCollection AddOsParser(
        this IServiceCollection services,
        Action<UaDetectorOptionsBuilder>? optionsAction = null
    )
    {
        var optionsBuilder = new UaDetectorOptionsBuilder();
        optionsAction?.Invoke(optionsBuilder);

        var osParser = new OsParser(optionsBuilder.Build());

        services.AddSingleton<IOsParser>(osParser);

        _ = Task.Run(() => osParser.TryParse(WarmupUserAgent, out _));

        return services;
    }

    public static IServiceCollection AddBrowserParser(
        this IServiceCollection services,
        Action<UaDetectorOptionsBuilder>? optionsAction = null
    )
    {
        var optionsBuilder = new UaDetectorOptionsBuilder();
        optionsAction?.Invoke(optionsBuilder);

        var browserParser = new BrowserParser(optionsBuilder.Build());

        services.AddSingleton<IBrowserParser>(browserParser);

        _ = Task.Run(() => browserParser.TryParse(WarmupUserAgent, out _));

        return services;
    }

    public static IServiceCollection AddClientParser(
        this IServiceCollection services,
        Action<UaDetectorOptionsBuilder>? optionsAction = null
    )
    {
        var optionsBuilder = new UaDetectorOptionsBuilder();
        optionsAction?.Invoke(optionsBuilder);

        var clientParser = new ClientParser(optionsBuilder.Build());

        services.AddSingleton<IClientParser>(clientParser);

        _ = Task.Run(() => clientParser.TryParse(WarmupUserAgent, out _));

        return services;
    }

    public static IServiceCollection AddBotParser(
        this IServiceCollection services,
        Action<BotParserOptionsBuilder>? optionsAction = null
    )
    {
        var optionsBuilder = new BotParserOptionsBuilder();
        optionsAction?.Invoke(optionsBuilder);

        var botParser = new BotParser(optionsBuilder.Build());

        services.AddSingleton<IBotParser>(botParser);

        _ = Task.Run(() => botParser.TryParse(WarmupUserAgent, out _));

        return services;
    }
}
