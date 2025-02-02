using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;

namespace ScriptBee.Gateway.Web.Tests;

public class TestWebApplicationFactory<TStartup>(ITestOutputHelper output, List<string> roles)
    : WebApplicationFactory<TStartup>
    where TStartup : class
{
    public static HttpClient CreateClient(ITestOutputHelper output, List<string> roles)
    {
        var factory = new TestWebApplicationFactory<TStartup>(output, roles);
        return factory.CreateClient();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(_ =>
        {
            // TODO: Add mocks
        });

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddProvider(new XUnitLoggerProvider(output));
            logging.AddDebug();
        });

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
