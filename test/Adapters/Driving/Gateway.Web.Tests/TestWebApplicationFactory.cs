using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScriptBee.Domain.Model.Authorization;
using Xunit.Abstractions;

namespace ScriptBee.Gateway.Web.Tests;

public class TestWebApplicationFactory<TStartup>(
    ITestOutputHelper output,
    List<UserRole> roles,
    Action<IServiceCollection>? configureDelegate = null)
    : WebApplicationFactory<TStartup>
    where TStartup : class
{
    public TestWebApplicationFactory(ITestOutputHelper output) : this(output, [])
    {
    }

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
        ConfigureServices(builder);
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
            { "Features:DisableAuthorization", "false" }
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

    private void ConfigureServices(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.TestAuthenticationScheme;
                options.DefaultChallengeScheme = TestAuthHandler.TestAuthenticationScheme;
            }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                TestAuthHandler.TestAuthenticationScheme, _ => { });

            services.AddScoped<TestAuthHandler>(provider => new TestAuthHandler(
                provider.GetRequiredService<IOptionsMonitor<AuthenticationSchemeOptions>>(),
                provider.GetRequiredService<ILoggerFactory>(),
                provider.GetRequiredService<UrlEncoder>()
            )
            {
                Roles = roles
            });
        });
    }
}
