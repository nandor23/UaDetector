using Microsoft.Extensions.DependencyInjection;
using UaDetector.Parsers;

namespace UaDetector;

public static class UaDetectorServiceCollectionExtensions
{
    public static IServiceCollection AddUaDetector(
        this IServiceCollection services,
        Action<UaDetectorOptionsBuilder>? optionsAction = null
    )
    {
        var optionsBuilder = new UaDetectorOptionsBuilder();
        optionsAction?.Invoke(optionsBuilder);

        services.AddSingleton<IUaDetector>(new UaDetector(optionsBuilder.Build()));
        return services;
    }

    public static IServiceCollection AddOsParser(
        this IServiceCollection services,
        Action<UaDetectorOptionsBuilder>? optionsAction = null
    )
    {
        var optionsBuilder = new UaDetectorOptionsBuilder();
        optionsAction?.Invoke(optionsBuilder);

        services.AddSingleton<IOsParser>(new OsParser(optionsBuilder.Build()));
        return services;
    }

    public static IServiceCollection AddBrowserParser(
        this IServiceCollection services,
        Action<UaDetectorOptionsBuilder>? optionsAction = null
    )
    {
        var optionsBuilder = new UaDetectorOptionsBuilder();
        optionsAction?.Invoke(optionsBuilder);

        services.AddSingleton<IBrowserParser>(new BrowserParser(optionsBuilder.Build()));
        return services;
    }

    public static IServiceCollection AddClientParser(
        this IServiceCollection services,
        Action<UaDetectorOptionsBuilder>? optionsAction = null
    )
    {
        var optionsBuilder = new UaDetectorOptionsBuilder();
        optionsAction?.Invoke(optionsBuilder);

        services.AddSingleton<IClientParser>(new ClientParser(optionsBuilder.Build()));
        return services;
    }

    public static IServiceCollection AddBotParser(
        this IServiceCollection services,
        Action<BotParserOptionsBuilder>? optionsAction = null
    )
    {
        var optionsBuilder = new BotParserOptionsBuilder();
        optionsAction?.Invoke(optionsBuilder);

        services.AddSingleton<IBotParser>(new BotParser(optionsBuilder.ParserOptions));
        return services;
    }
}
