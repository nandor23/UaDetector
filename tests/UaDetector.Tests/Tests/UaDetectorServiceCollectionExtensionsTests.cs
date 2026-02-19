using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using UaDetector.Parsers;

namespace UaDetector.Tests.Tests;

public class UaDetectorServiceCollectionExtensionsTests
{
    [Test]
    public void AddUaDetector_RegistersAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddUaDetector();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var detector1 = serviceProvider.GetRequiredService<IUaDetector>();
        var detector2 = serviceProvider.GetRequiredService<IUaDetector>();

        detector1.ShouldBe(detector2);
    }

    [Test]
    public void AddOsParser_RegistersAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOsParser();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var parser1 = serviceProvider.GetRequiredService<IOsParser>();
        var parser2 = serviceProvider.GetRequiredService<IOsParser>();

        parser1.ShouldBe(parser2);
    }

    [Test]
    public void AddBrowserParser_RegistersAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddBrowserParser();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var parser1 = serviceProvider.GetRequiredService<IBrowserParser>();
        var parser2 = serviceProvider.GetRequiredService<IBrowserParser>();

        parser1.ShouldBe(parser2);
    }

    [Test]
    public void AddClientParser_RegistersAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddClientParser();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var parser1 = serviceProvider.GetRequiredService<IClientParser>();
        var parser2 = serviceProvider.GetRequiredService<IClientParser>();

        parser1.ShouldBe(parser2);
    }

    [Test]
    public void AddBotParser_RegistersAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddBotParser();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var parser1 = serviceProvider.GetRequiredService<IBotParser>();
        var parser2 = serviceProvider.GetRequiredService<IBotParser>();

        parser1.ShouldBe(parser2);
    }
}
