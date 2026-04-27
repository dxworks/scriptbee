using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ScriptBee.Tests.Common;

public class TestWebApplicationFactory<TStartup>(
    ITestOutputHelper output,
    Action<IServiceCollection>? configureDelegate = null
) : WebApplicationFactory<TStartup>
    where TStartup : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        if (configureDelegate != null)
        {
            builder.ConfigureServices(configureDelegate);
        }

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IAntiforgery>();
            services.AddSingleton<IAntiforgery, NoOpAntiforgery>();
        });
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
        var configurationValues = new Dictionary<string, string?>
        {
            { "ScriptBee:Analysis:Driver", "Docker" },
            { "ScriptBee:Analysis:Image", "test-image" },
            { "ScriptBee:Analysis:Docker:DockerSocket", "unix:///var/run/docker.sock" },
        };
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

public class NoOpAntiforgery : IAntiforgery
{
    public AntiforgeryTokenSet GetAndStoreTokens(HttpContext httpContext) =>
        new("test", "test", "test", "test");

    public AntiforgeryTokenSet GetTokens(HttpContext httpContext) =>
        new("test", "test", "test", "test");

    public Task<bool> IsRequestValidAsync(HttpContext httpContext) => Task.FromResult(true);

    public void SetCookieTokenAndHeader(HttpContext httpContext) { }

    public Task ValidateRequestAsync(HttpContext httpContext) => Task.CompletedTask;
}
