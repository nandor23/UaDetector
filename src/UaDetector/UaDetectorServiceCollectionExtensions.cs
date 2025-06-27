using Microsoft.Extensions.DependencyInjection;

using UaDetector.Parsers;

namespace UaDetector;

public static class UaDetectorServiceCollectionExtensions
{
    private const string WarmupUserAgent = "service-warmup";

    public static IServiceCollection AddUaDetector(
        this IServiceCollection services,
        Action<UaDetectorOptionsBuilder>? optionsAction = null
    )
    {
        var optionsBuilder = new UaDetectorOptionsBuilder();
        optionsAction?.Invoke(optionsBuilder);

        var uaDetector = new UaDetector(optionsBuilder.Build());
        uaDetector.TryParse(WarmupUserAgent, out _);

        services.AddSingleton<IUaDetector>(uaDetector);
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
        osParser.TryParse(WarmupUserAgent, out _);

        services.AddSingleton<IOsParser>(osParser);
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
        browserParser.TryParse(WarmupUserAgent, out _);

        services.AddSingleton<IBrowserParser>(browserParser);
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
        clientParser.TryParse(WarmupUserAgent, out _);

        services.AddSingleton<IClientParser>(clientParser);
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
        botParser.TryParse(WarmupUserAgent, out _);

        services.AddSingleton<IBotParser>(botParser);
        return services;
    }
}
