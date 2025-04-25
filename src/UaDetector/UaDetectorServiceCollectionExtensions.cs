using Microsoft.Extensions.DependencyInjection;

using UaDetector.Parsers;

namespace UaDetector;

public static class UaDetectorServiceCollectionExtensions
{
    public static IServiceCollection AddUaDetector(
        this IServiceCollection services,
        Action<UaDetectorOptions>? configureOptions = null
    )
    {
        var options = new UaDetectorOptions();
        configureOptions?.Invoke(options);

        services.AddSingleton<IUaDetector>(new UaDetector(options));
        return services;
    }
    
    public static IServiceCollection AddOsParser(
        this IServiceCollection services,
        Action<ParserOptions>? configureOptions = null
    )
    {
        var options = new ParserOptions();
        configureOptions?.Invoke(options);

        services.AddSingleton<IOsParser>(new OsParser(options));
        return services;
    }
    
    public static IServiceCollection AddBrowserParser(
        this IServiceCollection services,
        Action<ParserOptions>? configureOptions = null
    )
    {
        var options = new ParserOptions();
        configureOptions?.Invoke(options);

        services.AddSingleton<IBrowserParser>(new BrowserParser(options));
        return services;
    }
    
    public static IServiceCollection AddClientParser(
        this IServiceCollection services,
        Action<ParserOptions>? configureOptions = null
    )
    {
        var options = new ParserOptions();
        configureOptions?.Invoke(options);

        services.AddSingleton<IClientParser>(new ClientParser(options));
        return services;
    }
    
    public static IServiceCollection AddBotParser(this IServiceCollection services)
    {
        services.AddSingleton<IBotParser>(new BotParser());
        return services;
    }
}
