using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScriptBee.Tests.Common;

namespace ScriptBee.Analysis.Web.Tests;

public class AnalysisTestWebApplicationFactory(
    ITestOutputHelper output,
    Action<IServiceCollection>? configureDelegate = null
) : TestWebApplicationFactory<Program>(output, configureDelegate)
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        var configurationValues = new Dictionary<string, string?>
        {
            { "ScriptBee:InstanceId", "74d9ae13-525c-4076-9b5b-53303a7ad547" },
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
