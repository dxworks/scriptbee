using DxWorks.Hub.Sdk;
using Microsoft.Extensions.DependencyInjection;

namespace ScriptBee.Marketplace.Client.Extensions;

public static class ScriptBeeMarketplaceClientExtensions
{
    public static IServiceCollection AddScriptBeeMarketplaceClient(
        this IServiceCollection services,
        Action<DxWorksHubSdkOptions>? configureOptionsAction = null
    )
    {
        return services
            .AddDxWorksHubSdk(configureOptionsAction)
            .AddSingleton<IMarketPluginFetcher, MarketPluginFetcher>()
            .AddSingleton<IPluginUrlFetcher, PluginUrlFetcher>();
    }
}
