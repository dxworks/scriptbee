using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace ScriptBee.Gateway.Web.Tests;

public class TestWebApplicationFactory<TStartup>(
    ITestOutputHelper output,
    Action<IServiceCollection>? configureDelegate = null)
    : WebApplicationFactory<TStartup>
    where TStartup : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        if (configureDelegate != null)
        {
            builder.ConfigureServices(configureDelegate);
        }

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ConfigureLoggingServices(builder);
        ConfigureConfigurations(builder);
    }

    private void ConfigureLoggingServices(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.Services.AddSingleton<ILoggerFactory, LoggerFactory>();
            logging.AddProvider(new XUnitLoggerProvider(output));
        });
    }

    private static void ConfigureConfigurations(IWebHostBuilder builder)
    {
        var configurationValues = new Dictionary<string, string?>();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationValues)
            .Build();

        builder
            .UseConfiguration(configuration)
            .ConfigureAppConfiguration(configurationBuilder =>
            {
                configurationBuilder.AddInMemoryCollection(configurationValues);
            });
    }
}
